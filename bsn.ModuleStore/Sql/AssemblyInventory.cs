// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using bsn.ModuleStore.Sql.Script;
using bsn.ModuleStore.Sql.Script.Tokens;

using NLog;

namespace bsn.ModuleStore.Sql {
	public class AssemblyInventory: InstallableInventory {
		private static readonly Dictionary<Assembly, AssemblyInventory> cachedInventories = new Dictionary<Assembly, AssemblyInventory>();
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public static AssemblyInventory Get(Assembly assembly) {
			if (assembly == null) {
				throw new ArgumentNullException(nameof(assembly));
			}
			lock (cachedInventories) {
				if (!cachedInventories.TryGetValue(assembly, out var result)) {
					result = new AssemblyInventory(new AssemblyHandle(assembly));
					cachedInventories.Add(assembly, result);
				}
				return result;
			}
		}

		private readonly IAssemblyHandle assembly;
		private readonly ReadOnlyCollection<KeyValuePair<SqlAssemblyAttribute, string>> attributes;
		private readonly List<SqlExceptionMappingAttribute> exceptionMappings = new List<SqlExceptionMappingAttribute>();
		private readonly HashSet<string> processedManifestStreamKeys = new HashSet<string>(StringComparer.Ordinal);
		private readonly int requiredEngineVersion = 9;
		private readonly SortedList<int, IScriptableStatement[]> updateStatements = new SortedList<int, IScriptableStatement[]>();
		private readonly int updateVersion;

		public AssemblyInventory(IAssemblyHandle assembly) {
			this.assembly = assembly;
			attributes = assembly.GetCustomAttributes<SqlAssemblyAttribute>().ToList().AsReadOnly();
			foreach (var attribute in attributes) {
				if (attribute.Key is SqlRequiredVersionAttribute requiredVersionAttribute) {
					if (requiredVersionAttribute.RequiredEngineVersion > requiredEngineVersion) {
						requiredEngineVersion = requiredVersionAttribute.RequiredEngineVersion;
					}
				} else {
					if (attribute.Key is SqlSetupScriptAttributeBase setupScriptAttribute) {
						using (var reader = OpenText(setupScriptAttribute, attribute.Value, out var manifestStreamKey)) {
							if (processedManifestStreamKeys.Add(manifestStreamKey)) {
								try {
									ProcessSingleScript(reader, AddAdditionalSetupStatement);
								} catch (ParseException ex) {
									ex.FileName = setupScriptAttribute.ManifestResourceName;
									throw;
								}
							}
						}
					} else {
						if (attribute.Key is SqlUpdateScriptAttribute updateScriptAttribute) {
							if (updateScriptAttribute.Version < 1) {
								var message = $"Update script versions must be at least 1, but {updateScriptAttribute.Version} was specified (script: {updateScriptAttribute.ManifestResourceName})";
								log.Error(message);
								throw new InvalidOperationException(message);
							}
							using (var reader = OpenText(updateScriptAttribute, attribute.Value)) {
								updateStatements.Add(updateScriptAttribute.Version, ScriptParser.Parse(reader).Cast<IScriptableStatement>().ToArray());
								updateVersion = Math.Max(updateVersion, updateScriptAttribute.Version);
							}
						} else {
							if (attribute.Key is SqlExceptionMappingAttribute exceptionMappingAttribute) {
								exceptionMappings.Add(exceptionMappingAttribute);
							} else {
								log.Warn("Unrecognized assembly SQL attribute {typeName}", attribute.Key.GetType().FullName);
							}
						}
					}
				}
			}
			var expectedVersion = 1;
			foreach (var update in updateStatements) {
				Debug.Assert(update.Key == expectedVersion);
				StatementSetSchemaOverride(update.Value);
				expectedVersion = update.Key+1;
			}
			AdditionalSetupStatementSetSchemaOverride();
			exceptionMappings.Sort((x, y) => x.ComputeSpecificity()-y.ComputeSpecificity());
		}

		public AssemblyName AssemblyName => assembly.AssemblyName;

		public ReadOnlyCollection<KeyValuePair<SqlAssemblyAttribute, string>> Attributes => attributes;

		public List<SqlExceptionMappingAttribute> ExceptionMappings => exceptionMappings;

		public int RequiredEngineVersion => requiredEngineVersion;

		public SortedList<int, IScriptableStatement[]> UpdateStatements => updateStatements;

		public int UpdateVersion => updateVersion;

		public IEnumerable<string> GenerateUpdateSql(DatabaseInventory inventory, int currentVersion) {
			if (inventory == null) {
				throw new ArgumentNullException(nameof(inventory));
			}
			SetQualification(inventory.SchemaName);
			inventory.SetQualification(inventory.SchemaName);
			try {
				var resolver = new DependencyResolver();
				var alterUsingUpdateScript = new List<IInstallStatement>();
				var dropStatements = new Dictionary<string, IScriptableStatement>(StringComparer.OrdinalIgnoreCase);
				var newObjectNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				var refreshObjectNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				refreshObjectNames.UnionWith(inventory.Objects.OfType<CreateViewStatement>().Where(v => !(v.ViewOption is OptionSchemabindingToken)).Select(v => v.ObjectName));
				// refreshObjectNames.UnionWith(inventory.Objects.OfType<CreateFunctionStatement>().Where(f => !(f.Option is OptionSchemabindingToken)).Select(f => f.ObjectName));
				foreach (var pair in Compare(inventory, this, inventory.TargetEngine)) {
					switch (pair.Value) {
					case InventoryObjectDifference.None:
						resolver.AddExistingObject(pair.Key.ObjectName);
						break;
					case InventoryObjectDifference.Different:
						if (pair.Key.AlterUsingUpdateScript) {
							alterUsingUpdateScript.Add(pair.Key);
						} else {
							if (pair.Key is AlterTableAddConstraintFragment alterConstraint) {
								dropStatements.Add(pair.Key.ObjectName, pair.Key.CreateDropStatement());
								resolver.Add(pair.Key);
							} else {
								if (pair.Key.DisableUsagesForUpdate) {
									Debug.Assert(!(pair.Key is CreateTableFragment)); // we must not wrap those - but they shouldn't return true for this flag
									resolver.Add(new DependencyDisablingAlterStatement(pair.Key.CreateAlterStatement()));
								} else {
									resolver.Add(pair.Key.CreateAlterStatement());
								}
							}
							refreshObjectNames.Remove(pair.Key.ObjectName);
						}
						break;
					case InventoryObjectDifference.SourceOnly:
						dropStatements.Add(pair.Key.ObjectName, pair.Key.CreateDropStatement());
						refreshObjectNames.Remove(pair.Key.ObjectName);
						break;
					case InventoryObjectDifference.TargetOnly:
						resolver.Add(pair.Key);
						if (pair.Key.IsPartOfSchemaDefinition) {
							newObjectNames.Add(pair.Key.ObjectName);
						}
						break;
					}
				}
				var builder = new StringBuilder(4096);
				// first drop table constraints and indices (if any)
				foreach (var dropStatement in dropStatements.Values.OfType<AlterTableDropConstraintStatement>()) {
					yield return WriteStatement(dropStatement, builder, inventory.TargetEngine);
				}
				foreach (var dropStatement in dropStatements.Values.OfType<DropIndexStatement>()) {
					yield return WriteStatement(dropStatement, builder, inventory.TargetEngine);
				}
				// now perform all possible actions which do not rely on tables which are altered
				var createdTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (var statement in resolver.GetInOrder(false)) {
					if (statement is CreateTableFragment createTable) {
						yield return WriteStatement(createTable.Owner.CreateStatementFragments(CreateFragmentMode.CreateOnExistingSchema).OfType<CreateTableFragment>().Single(), builder, inventory.TargetEngine);
						createdTables.Add(createTable.ObjectName);
					} else if (!statement.IsTableUniqueConstraintOfTables(createdTables)) {
						foreach (var innerStatement in HandleDependendObjects(statement, inventory, dropStatements.Keys)) {
							yield return WriteStatement(innerStatement, builder, inventory.TargetEngine);
						}
					}
				}
				// then perform updates (if any)
				var droppedTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (var update in updateStatements.Where(u => u.Key > currentVersion)) {
					foreach (var statement in update.Value) {
						if (statement is DropTableStatement dropTable) {
							droppedTables.Add(dropTable.ObjectName);
						}
						yield return WriteStatement(statement, builder, inventory.TargetEngine);
					}
				}
				// now that the update scripts have updated the tables, mark the tables in the dependency resolver
				foreach (var createTableStatement in alterUsingUpdateScript) {
					resolver.AddExistingObject(createTableStatement.ObjectName);
				}
				// refresh the views and functions
				foreach (var objectName in refreshObjectNames) {
					yield return $"EXEC [sp_refreshsqlmodule] '[{inventory.SchemaName}].[{objectName}]'";
				}
				// try to perform the remaining actions
				foreach (var statement in resolver.GetInOrder(true).Where(statement => !(statement.IsTableUniqueConstraintOfTables(createdTables) || statement.DependsOnTables(droppedTables)))) {
					foreach (var innerStatement in HandleDependendObjects(statement, inventory, dropStatements.Keys)) {
						yield return WriteStatement(innerStatement, builder, inventory.TargetEngine);
					}
				}
				// execute insert statements for table setup data
				if (AdditionalSetupStatements.Any()) {
					var disabledChecks = false;
					foreach (var statement in AdditionalSetupStatements) {
						Qualified<SchemaName, TableName> name = null;
						if (statement is InsertStatement insertStatement) {
							if (insertStatement.DestinationRowset is DestinationRowset<Qualified<SchemaName, TableName>> targetTable) {
								name = targetTable.Name;
							}
						} else {
							if (statement is SetIdentityInsertStatement setIdentityInsertStatement) {
								name = setIdentityInsertStatement.TableName;
							}
						}
						if ((name != null) && name.IsQualified && string.Equals(name.Qualification.Value, inventory.SchemaName, StringComparison.OrdinalIgnoreCase) && newObjectNames.Contains(name.Name.Value)) {
							if (!disabledChecks) {
								foreach (var table in Objects.OfType<CreateTableStatement>()) {
									yield return WriteStatement(new AlterTableNocheckConstraintStatement(table.TableName, new TableCheckToken()), builder, inventory.TargetEngine);
								}
								disabledChecks = true;
							}
							yield return WriteStatement(statement, builder, inventory.TargetEngine);
						}
					}
					if (disabledChecks) {
						foreach (var table in Objects.OfType<CreateTableStatement>()) {
							yield return WriteStatement(new AlterTableCheckConstraintStatement(table.TableName, new TableWithCheckToken()), builder, inventory.TargetEngine);
						}
					}
				}
				// finally drop objects which are no longer used
				foreach (var dropStatement in dropStatements.Values.Where(s => !(s is AlterTableDropConstraintStatement || s is DropTableStatement || s is DropIndexStatement || s.DependsOnTables(droppedTables)))) {
					yield return WriteStatement(dropStatement, builder, inventory.TargetEngine);
				}
				// refresh the SPs
				foreach (var objectName in Objects.OfType<CreateProcedureStatement>().Where(sp => !(sp.Option is OptionSchemabindingToken)).Select(sp => sp.ObjectName)) {
					yield return $"EXEC [sp_refreshsqlmodule] '[{inventory.SchemaName}].[{objectName}]'";
				}
			} finally {
				UnsetQualification();
				inventory.UnsetQualification();
			}
		}

		internal void AssertEngineVersion(int engineVersion) {
			if (engineVersion < RequiredEngineVersion) {
				log.Error("The assembly {assemblyName} requires a database engine version {requiredEngineVersion}, but the database engine version is {effectiveEngineVersion}", assembly.AssemblyName.FullName, requiredEngineVersion, engineVersion);
				throw new InvalidOperationException($"The assembly {assembly.AssemblyName.FullName} requires a database engine version {requiredEngineVersion}, but the database engine version is {engineVersion}");
			}
		}

		private IEnumerable<IInstallStatement> HandleDependendObjects(IInstallStatement statement, DatabaseInventory inventory, ICollection<string> droppedObjects) {
			if (statement is DependencyDisablingAlterStatement dependencyAltering) {
				var dependencyObjects = dependencyAltering.GetDependencyObjects(inventory, droppedObjects);
				foreach (var dependencyObject in dependencyObjects) {
					yield return dependencyObject.CreateDropStatement();
				}
				yield return statement;
				foreach (var dependencyObject in dependencyObjects) {
					// Take the new version of the object in order to avoid errors
					var newDependentObject = (IAlterableCreateStatement)Objects.SingleOrDefault(o => string.Equals(o.ObjectName, dependencyObject.ObjectName, StringComparison.OrdinalIgnoreCase));
					yield return newDependentObject ?? dependencyObject;
				}
			} else {
				yield return statement;
			}
		}

		private Stream OpenStream(SqlManifestResourceAttribute attribute, string optionalPrefix, out string manifestStreamKey) {
			if (attribute.ManifestResourceType == null) {
				manifestStreamKey = attribute.ManifestResourceName;
			} else {
				manifestStreamKey = attribute.ManifestResourceType.Namespace+Type.Delimiter+attribute.ManifestResourceName;
			}
			var result = assembly.GetManifestResourceStream(null, manifestStreamKey);
			if ((result == null) && (attribute.ManifestResourceType == null) && (!string.IsNullOrEmpty(optionalPrefix))) {
				manifestStreamKey = optionalPrefix+Type.Delimiter+attribute.ManifestResourceName;
				result = assembly.GetManifestResourceStream(null, manifestStreamKey);
			}
			if (result == null) {
				log.Error("The embedded SQL file {fileName} was not found", attribute.ManifestResourceName);
				throw new FileNotFoundException("The embedded SQL file was not found", attribute.ManifestResourceName);
			}
			return result;
		}

		private TextReader OpenText(SqlManifestResourceAttribute attribute, string optionalPrefix, out string manifestStreamKey) {
			return new StreamReader(OpenStream(attribute, optionalPrefix, out manifestStreamKey), true);
		}

		private TextReader OpenText(SqlManifestResourceAttribute attribute, string optionalPrefix) {
			return OpenText(attribute, optionalPrefix, out var key);
		}
	}
}

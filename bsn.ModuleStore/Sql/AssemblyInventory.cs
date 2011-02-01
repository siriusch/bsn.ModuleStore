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
//  
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public class AssemblyInventory: InstallableInventory {
		private readonly IAssemblyHandle assembly;
		private readonly ReadOnlyCollection<KeyValuePair<SqlAssemblyAttribute, string>> attributes;
		private readonly SortedList<int, Statement[]> updateStatements = new SortedList<int, Statement[]>();
		private readonly HashSet<string> processedManifestStreamKeys = new HashSet<string>(StringComparer.Ordinal);
		private readonly int updateVersion;

		public AssemblyInventory(Assembly assembly): this(new AssemblyHandle(assembly)) {}

		public AssemblyInventory(IAssemblyHandle assembly) {
			this.assembly = assembly;
			attributes = assembly.GetCustomAttributes<SqlAssemblyAttribute>().ToList().AsReadOnly();
			foreach (KeyValuePair<SqlAssemblyAttribute, string> attribute in attributes) {
				SqlSetupScriptAttributeBase setupScriptAttribute = attribute.Key as SqlSetupScriptAttributeBase;
				if (setupScriptAttribute != null) {
					string manifestStreamKey;
					using (TextReader reader = OpenText(setupScriptAttribute, attribute.Value, out manifestStreamKey)) {
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
					SqlUpdateScriptAttribute updateScriptAttribute = attribute.Key as SqlUpdateScriptAttribute;
					if (updateScriptAttribute != null) {
						if (updateScriptAttribute.Version < 1) {
							throw new InvalidOperationException(string.Format("Update script versions must be at least 1, but {0} was specified (script: {1})", updateScriptAttribute.Version, updateScriptAttribute.ManifestResourceName));
						}
						using (TextReader reader = OpenText(updateScriptAttribute, attribute.Value)) {
							updateStatements.Add(updateScriptAttribute.Version, ScriptParser.Parse(reader).ToArray());
							updateVersion = Math.Max(updateVersion, updateScriptAttribute.Version);
						}
					} else {
						Debug.WriteLine(attribute.Key.GetType(), "Unrecognized assembly SQL attribute");
					}
				}
			}
			int expectedVersion = 1;
			foreach (KeyValuePair<int, Statement[]> update in updateStatements) {
				Debug.Assert(update.Key == expectedVersion);
				StatementSetSchemaOverride(update.Value);
				expectedVersion = update.Key+1;
			}
			AdditionalSetupStatementSetSchemaOverride();
		}

		public AssemblyName AssemblyName {
			get {
				return assembly.AssemblyName;
			}
		}

		public ReadOnlyCollection<KeyValuePair<SqlAssemblyAttribute, string>> Attributes {
			get {
				return attributes;
			}
		}

		public SortedList<int, Statement[]> UpdateStatements {
			get {
				return updateStatements;
			}
		}
		public int UpdateVersion {
			get {
				return updateVersion;
			}
		}

		public IEnumerable<string> GenerateUpdateSql(DatabaseInventory inventory, int currentVersion) {
			if (inventory == null) {
				throw new ArgumentNullException("inventory");
			}
			SetQualification(inventory.SchemaName);
			inventory.SetQualification(inventory.SchemaName);
			try {
				DependencyResolver resolver = new DependencyResolver();
				List<CreateTableStatement> tables = new List<CreateTableStatement>();
				List<DropStatement> dropStatements = new List<DropStatement>();
				HashSet<string> newObjectNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (KeyValuePair<CreateStatement, InventoryObjectDifference> pair in Compare(inventory, this, inventory.TargetEngine)) {
					switch (pair.Value) {
					case InventoryObjectDifference.None:
						resolver.AddExistingObject(pair.Key.ObjectName);
						break;
					case InventoryObjectDifference.Different:
						CreateTableStatement createTable = pair.Key as CreateTableStatement;
						if (createTable != null) {
							tables.Add(createTable);
						} else {
							resolver.Add(pair.Key.ObjectName, pair.Key.CreateAlterStatement());
						}
						break;
					case InventoryObjectDifference.SourceOnly:
						dropStatements.Add(pair.Key.CreateDropStatement());
						break;
					case InventoryObjectDifference.TargetOnly:
						resolver.Add(pair.Key);
						switch (pair.Key.ObjectCategory) {
						case ObjectCategory.Table:
						case ObjectCategory.View:
							newObjectNames.Add(pair.Key.ObjectName);
							break;
						}
						break;
					}
				}
				StringBuilder builder = new StringBuilder(4096);
				// first perform all possible actions which do not rely on tables which are altered
				foreach (Statement statement in resolver.GetInOrder(false)) {
					yield return WriteStatement(statement, builder, inventory.TargetEngine);
				}
				// then perform updates (if any)
				foreach (KeyValuePair<int, Statement[]> update in updateStatements.Where(u => u.Key > currentVersion)) {
					foreach (Statement statement in update.Value) {
						yield return WriteStatement(statement, builder, inventory.TargetEngine);
					}
				}
				// now that the update scripts have updated the tables, mark the tables in the dependency resolver
				foreach (CreateTableStatement createTableStatement in tables) {
					resolver.AddExistingObject(createTableStatement.ObjectName);
				}
				// try to perform the remaining actions
				foreach (Statement statement in resolver.GetInOrder(true)) {
					yield return WriteStatement(statement, builder, inventory.TargetEngine);
				}
				// execute insert statements for table setup data
				foreach (InsertStatement insertStatement in AdditionalSetupStatements.OfType<InsertStatement>()) {
					DestinationRowset<Qualified<SchemaName, TableName>> targetTable = insertStatement.DestinationRowset as DestinationRowset<Qualified<SchemaName, TableName>>;
					if (targetTable != null) {
						Qualified<SchemaName, TableName> name = targetTable.Name;
						if (name.IsQualified && string.Equals(name.Qualification.Value, inventory.SchemaName, StringComparison.OrdinalIgnoreCase) && newObjectNames.Contains(name.Name.Value)) {
							yield return WriteStatement(insertStatement, builder, inventory.TargetEngine);
						}
					}
				}
				// finally drop objects which are no longer used
				foreach (DropStatement dropStatement in dropStatements) {
					yield return WriteStatement(dropStatement, builder, inventory.TargetEngine);
				}
			} finally {
				UnsetQualification();
				inventory.UnsetQualification();
			}
		}

		private Stream OpenStream(SqlManifestResourceAttribute attribute, string optionalPrefix, out string manifestStreamKey) {
			if (attribute.ManifestResourceType == null) {
				manifestStreamKey = attribute.ManifestResourceName;
			} else {
				manifestStreamKey = attribute.ManifestResourceType.Namespace+Type.Delimiter+attribute.ManifestResourceName;
			}
			Stream result = assembly.GetManifestResourceStream(null, manifestStreamKey);
			if ((result == null) && (attribute.ManifestResourceType == null) && (!string.IsNullOrEmpty(optionalPrefix))) {
				manifestStreamKey = optionalPrefix+Type.Delimiter+attribute.ManifestResourceName;
				result = assembly.GetManifestResourceStream(null, manifestStreamKey);
				if (result == null) {
					throw new FileNotFoundException("The embedded SQL file was not found", attribute.ManifestResourceName);
				}
			}
			return result;
		}

		private TextReader OpenText(SqlManifestResourceAttribute attribute, string optionalPrefix, out string manifestStreamKey) {
			return new StreamReader(OpenStream(attribute, optionalPrefix, out manifestStreamKey), true);
		}

		private TextReader OpenText(SqlManifestResourceAttribute attribute, string optionalPrefix) {
			string key;
			return OpenText(attribute, optionalPrefix, out key);
		}
	}
}

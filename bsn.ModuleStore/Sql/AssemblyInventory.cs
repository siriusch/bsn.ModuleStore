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
		private readonly int setupUpdateVersion;
		private readonly SortedList<int, Statement[]> updateStatements = new SortedList<int, Statement[]>();

		public AssemblyInventory(Assembly assembly): this(new AssemblyHandle(assembly)) {}

		public AssemblyInventory(IAssemblyHandle assembly) {
			this.assembly = assembly;
			attributes = assembly.GetCustomAttributes<SqlAssemblyAttribute>().ToList().AsReadOnly();
			foreach (KeyValuePair<SqlAssemblyAttribute, string> attribute in attributes) {
				SqlSetupScriptAttributeBase setupScriptAttribute = attribute.Key as SqlSetupScriptAttributeBase;
				if (setupScriptAttribute != null) {
					using (TextReader reader = OpenText(setupScriptAttribute, attribute.Value)) {
						ProcessSingleScript(reader, AddAdditionalSetupStatement);
					}
				} else {
					SqlUpdateScriptAttribute updateScriptAttribute = attribute.Key as SqlUpdateScriptAttribute;
					if (updateScriptAttribute != null) {
						using (TextReader reader = OpenText(setupScriptAttribute, attribute.Value)) {
							updateStatements.Add(updateScriptAttribute.Version, ScriptParser.Parse(reader).ToArray());
							setupUpdateVersion = Math.Max(setupUpdateVersion, updateScriptAttribute.Version);
						}
					} else {
						Debug.WriteLine(attribute.Key.GetType(), "Unrecognized assembly SQL attribute");
					}
				}
			}
			foreach (Statement[] statements in updateStatements.Values) {
				StatementSetSchemaOverride(statements);
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

		public int SetupUpdateVersion {
			get {
				return setupUpdateVersion;
			}
		}

		public SortedList<int, Statement[]> UpdateStatements {
			get {
				return updateStatements;
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
				foreach (KeyValuePair<CreateStatement, InventoryObjectDifference> pair in Compare(inventory, this)) {
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
						break;
					}
				}
				StringBuilder builder = new StringBuilder(4096);
				// first perform all possible actions which do not rely on tables which are altered
				foreach (Statement statement in resolver.GetInOrder(false)) {
					yield return WriteStatement(statement, builder);
				}
				// then perform updates (if any)
				foreach (KeyValuePair<int, Statement[]> update in updateStatements.Where(u => u.Key > currentVersion)) {
					foreach (Statement statement in update.Value) {
						yield return WriteStatement(statement, builder);
					}
				}
				// now that the update scripts have updated the tables, mark the tables in the dependency resolver
				foreach (CreateTableStatement createTableStatement in tables) {
					resolver.AddExistingObject(createTableStatement.ObjectName);
				}
				// try to perform the remaining actions
				foreach (Statement statement in resolver.GetInOrder(true)) {
					yield return WriteStatement(statement, builder);
				}
				// finally drop objects which are no longer used
				foreach (DropStatement dropStatement in dropStatements) {
					yield return WriteStatement(dropStatement, builder);
				}
			} finally {
				UnsetQualification();
				inventory.UnsetQualification();
			}
		}

		private Stream OpenStream(SqlManifestResourceAttribute attribute, string optionalPrefix) {
			Stream result = assembly.GetManifestResourceStream(attribute.ManifestResourceType, attribute.ManifestResourceName);
			if ((result == null) && (attribute.ManifestResourceType == null) && (!string.IsNullOrEmpty(optionalPrefix))) {
				result = assembly.GetManifestResourceStream(null, optionalPrefix+Type.Delimiter+attribute.ManifestResourceName);
				if (result == null) {
					throw new FileNotFoundException("The embedded SQL file was not found", attribute.ManifestResourceName);
				}
			}
			return result;
		}

		private TextReader OpenText(SqlManifestResourceAttribute attribute, string optionalPrefix) {
			return new StreamReader(OpenStream(attribute, optionalPrefix), true);
		}
	}
}
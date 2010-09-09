using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public abstract class InstallableInventory: Inventory {
		private readonly List<Statement> additionalSetupStatements = new List<Statement>();

		protected void AddAdditionalSetupStatement(Statement statement) {
			if (statement == null) {
				throw new ArgumentNullException("statement");
			}
			additionalSetupStatements.Add(statement);
		}

		public IEnumerable<Statement> AdditionalSetupStatements {
			get {
				return additionalSetupStatements;
			}
		}

		protected void AdditionalSetupStatementSchemaFixup() {
			StatementSetSchemaOverride(additionalSetupStatements);
		}

		protected void StatementSetSchemaOverride(IEnumerable<Statement> statements) {
			foreach (Statement statement in statements) {
				foreach (IQualifiedName<SchemaName> name in statement.GetInnerSchemaQualifiedNames(n => ObjectSchemas.Contains(n))) {
					name.SetOverride(this);
				}
			}
		}
		/*
		public IEnumerable<string> GenerateUpdateSql(DatabaseInventory inventory, int currentVersion) {
			if (inventory == null) {
				throw new ArgumentNullException("inventory");
			}
			SchemaName nameQualification = new SchemaName(inventory.SchemaName);
			DependencyResolver createResolver = new DependencyResolver();
			List<Statement> alterStatements = new List<Statement>();
			List<DropStatement> dropStatements = new List<DropStatement>();
			List<CreateStatement> qualifiedStatements = new List<CreateStatement>();
			foreach (KeyValuePair<CreateStatement, InventoryObjectDifference> pair in Compare(inventory, this)) {
				if (pair.Value == InventoryObjectDifference.None) {
					createResolver.AddExistingObject(pair.Key.ObjectName);
				} else {
					pair.Key.ObjectSchema = inventory.SchemaName;
					qualifiedStatements.Add(pair.Key);
					switch (pair.Value) {
					case InventoryObjectDifference.None:
						createResolver.AddExistingObject(pair.Key.ObjectName);
						break;
					case InventoryObjectDifference.Different:
						createResolver.AddExistingObject(pair.Key.ObjectName);
						if (pair.Key.ObjectCategory != ObjectCategory.Table) {
							alterStatements.Add(pair.Key.CreateAlterStatement());
						}
						break;
					case InventoryObjectDifference.SourceOnly:
						createResolver.Add(pair.Key);
						break;
					case InventoryObjectDifference.TargetOnly:
						dropStatements.Add(pair.Key.CreateDropStatement());
						break;
					}
				}
			}
			foreach (DropStatement dropStatement in dropStatements) {
				
			}
		}
		*/
		public IEnumerable<string> GenerateInstallSql(string schemaName) {
			if (string.IsNullOrEmpty(schemaName)) {
				throw new ArgumentNullException("schemaName");
			}
			DependencyResolver resolver = new DependencyResolver();
			SetQualification(null);
			try {
				using (StringWriter writer = new StringWriter()) {
					SqlWriter sqlWriter = new SqlWriter(writer);
					sqlWriter.Write("CREATE SCHEMA");
					sqlWriter.IncreaseIndent();
					sqlWriter.WriteScript(new SchemaName(schemaName), WhitespacePadding.SpaceBefore);
					foreach (CreateStatement statement in Objects) {
						if ((statement is CreateTableStatement) || (statement is CreateViewStatement)) {
							resolver.AddExistingObject(statement.ObjectName);
							sqlWriter.WriteLine();
							statement.WriteTo(sqlWriter);
						} else {
							resolver.Add(statement);
						}
					}
					sqlWriter.DecreaseIndent();
					yield return writer.ToString();
				}
				UnsetQualification();
				SetQualification(schemaName);
				StringBuilder builder = new StringBuilder(4096);
				foreach (CreateStatement statement in resolver.GetInOrder()) {
					builder.Length = 0;
					using (StringWriter writer = new StringWriter(builder)) {
						statement.WriteTo(new SqlWriter(writer));
					}
					yield return builder.ToString();
				}
				foreach (Statement additionalSetupStatement in AdditionalSetupStatements) {
					builder.Length = 0;
					using (StringWriter writer = new StringWriter(builder)) {
						additionalSetupStatement.WriteTo(new SqlWriter(writer));
					}
					yield return builder.ToString();
				}
			} finally {
				UnsetQualification();
			}
		}
	}
}
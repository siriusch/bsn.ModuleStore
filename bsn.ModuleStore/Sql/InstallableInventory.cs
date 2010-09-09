using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public abstract class InstallableInventory: Inventory {
		private readonly List<Statement> additionalSetupStatements = new List<Statement>();

		public IEnumerable<Statement> AdditionalSetupStatements {
			get {
				return additionalSetupStatements;
			}
		}

		public IEnumerable<string> GenerateInstallSql(string schemaName) {
			if (string.IsNullOrEmpty(schemaName)) {
				throw new ArgumentNullException("schemaName");
			}
			DependencyResolver resolver = new DependencyResolver();
			SetQualification(null);
			try {
				StringBuilder builder = new StringBuilder(4096);
				using (StringWriter writer = new StringWriter(builder)) {
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
				foreach (Statement statement in resolver.GetInOrder(true)) {
					yield return WriteStatement(statement, builder);
				}
				foreach (Statement additionalSetupStatement in AdditionalSetupStatements) {
					yield return WriteStatement(additionalSetupStatement, builder);
				}
			} finally {
				UnsetQualification();
			}
		}

		protected void AddAdditionalSetupStatement(Statement statement) {
			if (statement == null) {
				throw new ArgumentNullException("statement");
			}
			additionalSetupStatements.Add(statement);
		}

		protected void AdditionalSetupStatementSetSchemaOverride() {
			StatementSetSchemaOverride(additionalSetupStatements);
		}

		protected void StatementSetSchemaOverride(IEnumerable<Statement> statements) {
			foreach (Statement statement in statements) {
				foreach (IQualifiedName<SchemaName> name in statement.GetInnerSchemaQualifiedNames(n => ObjectSchemas.Contains(n))) {
					name.SetOverride(this);
				}
			}
		}
	}
}
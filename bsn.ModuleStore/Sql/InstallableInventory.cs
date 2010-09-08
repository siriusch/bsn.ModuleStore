using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public abstract class InstallableInventory: Inventory {
		private readonly Dictionary<Statement, ICollection<IQualifiedName<SchemaName>>> additionalSetupStatements = new Dictionary<Statement, ICollection<IQualifiedName<SchemaName>>>();

		protected void AddAdditionalSetupStatement(Statement statement) {
			if (statement == null) {
				throw new ArgumentNullException("statement");
			}
			additionalSetupStatements.Add(statement, new List<IQualifiedName<SchemaName>>(1));
		}

		public Dictionary<Statement, ICollection<IQualifiedName<SchemaName>>> AdditionalSetupStatements {
			get {
				return additionalSetupStatements;
			}
		}

		protected void AdditionalSetupStatementSchemaFixup() {
			foreach (KeyValuePair<Statement, ICollection<IQualifiedName<SchemaName>>> additionalSetupStatement in additionalSetupStatements) {
				if (additionalSetupStatement.Value.Count == 0) {
					foreach (IQualifiedName<SchemaName> name in additionalSetupStatement.Key.GetInnerSchemaQualifiedNames(n => ObjectSchemas.Contains(n))) {
						additionalSetupStatement.Value.Add(name);
						name.Qualification = null;
					}
				}
			}
		}

		public IEnumerable<string> GenerateInstallSql(string schemaName) {
			if (string.IsNullOrEmpty(schemaName)) {
				throw new ArgumentNullException("schemaName");
			}
			SchemaName nameQualification = new SchemaName(schemaName);
			DependencyResolver resolver = new DependencyResolver();
			using (StringWriter writer = new StringWriter()) {
				SqlWriter sqlWriter = new SqlWriter(writer);
				sqlWriter.Write("CREATE SCHEMA");
				sqlWriter.IncreaseIndent();
				sqlWriter.WriteScript(nameQualification, WhitespacePadding.SpaceBefore);
				foreach (CreateStatement statement in Objects) {
					if ((statement is CreateTableStatement) || (statement is CreateViewStatement)) {
						statement.ObjectSchema = null;
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
			StringBuilder builder = new StringBuilder(4096);
			foreach (CreateStatement statement in resolver.GetInOrder()) {
				builder.Length = 0;
				using (StringWriter writer = new StringWriter(builder)) {
					SqlWriter sqlWriter = new SqlWriter(writer);
					statement.ObjectSchema = schemaName;
					try {
						statement.WriteTo(sqlWriter);
					} finally {
						statement.ObjectSchema = null;
					}
				}
				yield return builder.ToString();
			}
			foreach (KeyValuePair<Statement, ICollection<IQualifiedName<SchemaName>>> additionalSetupStatement in AdditionalSetupStatements) {
				builder.Length = 0;
				using (StringWriter writer = new StringWriter(builder)) {
					SqlWriter sqlWriter = new SqlWriter(writer);
					foreach (IQualifiedName<SchemaName> name in additionalSetupStatement.Value) {
						name.Qualification = nameQualification;
					}
					try {
						additionalSetupStatement.Key.WriteTo(sqlWriter);
					} finally {
						foreach (IQualifiedName<SchemaName> name in additionalSetupStatement.Value) {
							name.Qualification = null;
						}
					}
				}
				yield return builder.ToString();
			}
		}
	}
}
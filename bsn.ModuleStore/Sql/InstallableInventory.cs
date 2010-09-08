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
			DependencyResolver resolver = new DependencyResolver();
			foreach (CreateStatement statement in Objects) {
				resolver.Add(statement);
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
			SchemaName nameQualification = string.IsNullOrEmpty(schemaName) ? null : new SchemaName(schemaName);
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
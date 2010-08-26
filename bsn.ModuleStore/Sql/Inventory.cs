// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.IO;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public abstract class Inventory {
		private readonly List<IQualifiedName<SchemaName>> names = new List<IQualifiedName<SchemaName>>();
		private readonly SortedDictionary<string, CreateStatement> objects = new SortedDictionary<string, CreateStatement>(StringComparer.OrdinalIgnoreCase);

		public abstract bool SchemaExists {
			get;
		}

		public virtual string SchemaName {
			get {
				return string.Empty;
			}
		}

		public void Dump(string schemaName, TextWriter writer) {
			SchemaName qualification = string.IsNullOrEmpty(schemaName) ? null : new SchemaName(schemaName);
			SetSchemaName(qualification);
			try {
				SqlWriter sqlWriter = new SqlWriter(writer);
				foreach (CreateStatement statement in objects.Values) {
					statement.WriteTo(sqlWriter);
					writer.WriteLine(";");
				}
			} finally {
				SetSchemaName(null);
			}
		}

		public virtual void Populate() {
			objects.Clear();
			names.Clear();
		}

		protected void AddObject(CreateStatement createStatement) {
			if (createStatement == null) {
				throw new ArgumentNullException("createStatement");
			}
			createStatement.ResetHash();
			createStatement.GetHash();
			objects.Add(createStatement.ObjectName, createStatement);
		}

		protected void AddSchemaQualifiedName(IQualifiedName<SchemaName> schemaQualifiedName) {
			if (schemaQualifiedName == null) {
				throw new ArgumentNullException("schemaQualifiedName");
			}
			schemaQualifiedName.Qualification = null;
			schemaQualifiedName.ResetHash();
			schemaQualifiedName.GetHash();
			names.Add(schemaQualifiedName);
		}

		protected void ProcessSingleScript(TextReader scriptReader, ref CreateTableStatement createTable, Action<Statement> unsupportedStatementFound) {
			ICollection<IQualifiedName<SchemaName>> names;
			List<CreateStatement> objects = new List<CreateStatement>();
			foreach (Statement statement in ScriptParser.Parse(scriptReader, out names)) {
				if (!((statement is SetOptionStatement) || (statement is AlterTableCheckConstraintStatementBase))) {
					AlterTableAddStatement addToTable = statement as AlterTableAddStatement;
					if (addToTable != null) {
						if ((createTable == null) || (!createTable.TableName.Name.Equals(addToTable.TableName.Name))) {
							throw DatabaseInventory.CreateException("Statement tries to modify another table:", statement);
						}
						createTable.Definitions.AddRange(addToTable.Definitions);
					} else {
						if (!(statement is CreateStatement)) {
							if (unsupportedStatementFound != null) {
								unsupportedStatementFound(statement);
							}
						}
						if (statement is CreateTableStatement) {
							createTable = (CreateTableStatement)statement;
						}
						objects.Add((CreateStatement)statement);
					}
				}
			}
			foreach (IQualifiedName<SchemaName> qualifiedName in names) {
				if (qualifiedName.IsQualified && qualifiedName.Qualification.Value.Equals(SchemaName, StringComparison.OrdinalIgnoreCase)) {
					AddSchemaQualifiedName(qualifiedName);
				}
			}
			foreach (CreateStatement statement in objects) {
				AddObject(statement);
			}
		}

		private void SetSchemaName(SchemaName qualification) {
			foreach (IQualifiedName<SchemaName> name in names) {
				name.Qualification = qualification;
			}
		}
	}
}
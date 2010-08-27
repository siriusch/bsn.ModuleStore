// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public abstract class Inventory {
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
			SqlWriter sqlWriter = new SqlWriter(writer);
			foreach (CreateStatement statement in objects.Values) {
				statement.ObjectSchema = schemaName;
				try {
					statement.WriteTo(sqlWriter);
					writer.WriteLine(";");
				} finally {
					statement.ObjectSchema = null;
				}
			}
		}

		public virtual void Populate() {
			objects.Clear();
		}

		protected void AddObject(CreateStatement createStatement) {
			if (createStatement == null) {
				throw new ArgumentNullException("createStatement");
			}
			createStatement.ResetHash();
			createStatement.GetHash();
			objects.Add(createStatement.ObjectName, createStatement);
		}

		protected void ProcessSingleScript(TextReader scriptReader, ref CreateTableStatement createTable, Action<Statement> unsupportedStatementFound) {
			List<CreateStatement> objects = new List<CreateStatement>();
			foreach (Statement statement in ScriptParser.Parse(scriptReader)) {
				if (!((statement is SetOptionStatement) || (statement is AlterTableCheckConstraintStatementBase))) {
					AlterTableAddStatement addToTable = statement as AlterTableAddStatement;
					if (addToTable != null) {
						if ((createTable == null) || (!createTable.TableName.Name.Equals(addToTable.TableName.Name))) {
							throw DatabaseInventory.CreateException("Statement tries to modify another table:", statement);
						}
						createTable.Definitions.AddRange(addToTable.Definitions);
					} else {
						CreateStatement createStatement = statement as CreateStatement;
						if (createStatement == null) {
							if (unsupportedStatementFound != null) {
								unsupportedStatementFound(statement);
							}
						} else {
							if (createStatement is CreateTableStatement) {
								createTable = (CreateTableStatement)createStatement;
							}
							createStatement.ObjectSchema = null;
							objects.Add(createStatement);
						}
					}
				}
			}
			foreach (CreateStatement statement in objects) {
				AddObject(statement);
			}
		}
	}
}
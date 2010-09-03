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

		public ICollection<CreateStatement> Objects {
			get {
				return objects.Values;
			}
		}

		public void Dump(string schemaName, TextWriter writer) {
			writer.WriteLine("-- Inventory hash: {0}", BitConverter.ToString(GetInventoryHash()).Replace("-", ""));
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

		public byte[] GetInventoryHash() {
			using (HashWriter writer = new HashWriter()) {
				SqlWriter sqlWriter = new SqlWriter(writer);
				foreach (CreateStatement statement in objects.Values) {
					Debug.Assert(string.IsNullOrEmpty(statement.ObjectSchema));
					statement.WriteTo(sqlWriter);
				}
				return writer.ToArray();
			}
		}

		protected void AddObject(CreateStatement createStatement) {
			if (createStatement == null) {
				throw new ArgumentNullException("createStatement");
			}
			createStatement.ResetHash();
			createStatement.GetHash();
			objects.Add(createStatement.ObjectName, createStatement);
		}

		protected void ProcessSingleScript(TextReader scriptReader, Action<Statement> unsupportedStatementFound) {
			List<CreateStatement> objects = new List<CreateStatement>();
			CreateTableStatement createTable = null;
			foreach (Statement statement in ScriptParser.Parse(scriptReader)) {
				if (!((statement is SetOptionStatement) || (statement is AlterTableCheckConstraintStatementBase))) {
					AlterTableAddStatement addToTable = statement as AlterTableAddStatement;
					if (addToTable != null) {
						if ((createTable == null) || (!createTable.TableName.Name.Equals(addToTable.TableName.Name))) {
							throw DatabaseInventory.CreateException("Statement tries to modify another table:", statement);
						}
						createTable.Definitions.AddRange(addToTable.Definitions);
						createTable.ResetSchemaNames();
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
							objects.Add(createStatement);
						}
					}
				}
			}
			foreach (CreateStatement statement in objects) {
				statement.ObjectSchema = null;
				AddObject(statement);
			}
		}
	}
}
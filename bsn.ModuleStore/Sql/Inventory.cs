// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public abstract class Inventory: IQualified<SchemaName> {
		private static readonly byte[] hashXor = new byte[] {0xDA, 0x39, 0xA3, 0xEE, 0x5E, 0x6B, 0x4B, 0x0D, 0x32, 0x55, 0xBF, 0xEF, 0x95, 0x60, 0x18, 0x90, 0xAF, 0xD8, 0x07, 0x09};

		public static IEnumerable<KeyValuePair<CreateStatement, InventoryObjectDifference>> Compare(Inventory source, Inventory target) {
			if (source == null) {
				throw new ArgumentNullException("source");
			}
			if (target == null) {
				throw new ArgumentNullException("target");
			}
			using (IEnumerator<CreateStatement> sourceEnumerator = source.Objects.GetEnumerator()) {
				using (IEnumerator<CreateStatement> targetEnumerator = target.Objects.GetEnumerator()) {
					bool hasSource = sourceEnumerator.MoveNext();
					bool hasTarget = targetEnumerator.MoveNext();
					while (hasSource && hasTarget) {
						CreateStatement sourceStatement = sourceEnumerator.Current;
						Debug.Assert(sourceStatement != null);
						CreateStatement targetStatement = targetEnumerator.Current;
						Debug.Assert(targetStatement != null);
						int diff = string.Compare(sourceStatement.ObjectName, targetStatement.ObjectName, StringComparison.OrdinalIgnoreCase);
						if (diff < 0) {
							yield return new KeyValuePair<CreateStatement, InventoryObjectDifference>(sourceStatement, InventoryObjectDifference.SourceOnly);
							hasSource = sourceEnumerator.MoveNext();
						} else if (diff > 0) {
							yield return new KeyValuePair<CreateStatement, InventoryObjectDifference>(targetStatement, InventoryObjectDifference.TargetOnly);
							hasTarget = targetEnumerator.MoveNext();
						} else {
							yield return new KeyValuePair<CreateStatement, InventoryObjectDifference>(targetStatement, targetStatement.Equals(sourceStatement) ? InventoryObjectDifference.None : InventoryObjectDifference.Different);
							hasSource = sourceEnumerator.MoveNext();
							hasTarget = targetEnumerator.MoveNext();
						}
					}
					while (hasSource) {
						yield return new KeyValuePair<CreateStatement, InventoryObjectDifference>(sourceEnumerator.Current, InventoryObjectDifference.SourceOnly);
						hasSource = sourceEnumerator.MoveNext();
					}
					while (hasTarget) {
						yield return new KeyValuePair<CreateStatement, InventoryObjectDifference>(targetEnumerator.Current, InventoryObjectDifference.TargetOnly);
						hasTarget = targetEnumerator.MoveNext();
					}
				}
			}
		}

		protected static string WriteStatement(Statement statement, bool emitComments, StringBuilder buffer) {
			buffer.Length = 0;
			using (StringWriter writer = new StringWriter(buffer)) {
				statement.WriteTo(new SqlWriter(writer, emitComments));
			}
			return buffer.ToString();
		}

		private readonly HashSet<string> objectSchemas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private readonly SortedDictionary<string, CreateStatement> objects = new SortedDictionary<string, CreateStatement>(StringComparer.OrdinalIgnoreCase);
		private readonly Stack<SchemaName> qualificationStack = new Stack<SchemaName>(4);

		protected Inventory() {
			qualificationStack.Push(null);
		}

		public bool IsEmpty {
			get {
				return objects.Count == 0;
			}
		}

		public T Find<T>(string objectName) where T: CreateStatement {
			T result = FindInternal<T>(objectName);
			if (result == null) {
				throw new ArgumentException(string.Format("The {0} object [{1}] does not exist", typeof(T).Name, objectName), "objectName");
			}
			return result;
		}

		private T FindInternal<T>(string objectName) where T: CreateStatement {
			if (objectName == null) {
				throw new ArgumentNullException("objectName");
			}
			CreateStatement statement;
			if (objects.TryGetValue(objectName, out statement)) {
				return statement as T;
			}
			return null;
		}

		public bool TryFind<T>(string objectName, out T result) where T: CreateStatement {
			result = FindInternal<T>(objectName);
			return result != null;
		}

		public ICollection<CreateStatement> Objects {
			get {
				return objects.Values;
			}
		}

		protected internal string ObjectSchema {
			get {
				SchemaName qualification = qualificationStack.Peek();
				return (qualification == null) ? string.Empty : qualification.Value;
			}
		}

		protected HashSet<string> ObjectSchemas {
			get {
				return objectSchemas;
			}
		}

		public void Dump(string schemaName, TextWriter writer) {
			writer.WriteLine("-- Inventory hash: {0}", BitConverter.ToString(GetInventoryHash()).Replace("-", ""));
			SqlWriter sqlWriter = new SqlWriter(writer);
			SetQualification(schemaName);
			try {
				foreach (CreateStatement statement in objects.Values) {
					statement.WriteTo(sqlWriter);
					writer.WriteLine(";");
				}
			} finally {
				UnsetQualification();
			}
		}

		public byte[] GetInventoryHash() {
			SetQualification(null);
			try {
				using (HashWriter writer = new HashWriter()) {
					SqlWriter sqlWriter = new SqlWriter(writer, false);
					foreach (CreateStatement statement in objects.Values) {
						statement.WriteTo(sqlWriter);
					}
					byte[] inventoryHash = writer.ToArray();
					for (int i = 0; i < hashXor.Length; i++) {
						inventoryHash[i] ^= hashXor[i];
					}
					return inventoryHash;
				}
			} finally {
				UnsetQualification();
			}
		}

		public bool IsSameInventoryHash(byte[] inventoryHash) {
			if (inventoryHash == null) {
				throw new ArgumentNullException("inventoryHash");
			}
			return HashWriter.HashEqual(GetInventoryHash(), inventoryHash);
		}

		protected internal void SetQualification(string schemaName) {
			qualificationStack.Push(string.IsNullOrEmpty(schemaName) ? null : new SchemaName(schemaName));
		}

		protected internal void UnsetQualification() {
			qualificationStack.Pop();
		}

		protected void AddObject(CreateStatement createStatement) {
			if (createStatement == null) {
				throw new ArgumentNullException("createStatement");
			}
			objectSchemas.Add(createStatement.ObjectSchema);
			foreach (IQualifiedName<SchemaName> qualifiedName in createStatement.GetObjectSchemaQualifiedNames()) {
				qualifiedName.SetOverride(this);
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
				AddObject(statement);
			}
		}

		SchemaName IQualified<SchemaName>.Qualification {
			get {
				return qualificationStack.Peek();
			}
		}
	}
}
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using NLog;

using bsn.GoldParser.Text;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public abstract class Inventory: IQualified<SchemaName> {
		private static readonly byte[] hashXor = new byte[] {0xDA, 0x39, 0xA3, 0xEE, 0x5E, 0x6B, 0x4B, 0x0D, 0x32, 0x55, 0xBF, 0xEF, 0x95, 0x60, 0x18, 0x90, 0xAF, 0xD8, 0x07, 0x09};
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public static IEnumerable<KeyValuePair<IAlterableCreateStatement, InventoryObjectDifference>> Compare(Inventory source, Inventory target, DatabaseEngine engine) {
			if (source == null) {
				throw new ArgumentNullException(nameof(source));
			}
			if (target == null) {
				throw new ArgumentNullException(nameof(target));
			}
			using (var sourceEnumerator = GetOrderedFragments(source, engine).GetEnumerator()) {
				using (var targetEnumerator = GetOrderedFragments(target, engine).GetEnumerator()) {
					var hasSource = sourceEnumerator.MoveNext();
					var hasTarget = targetEnumerator.MoveNext();
					while (hasSource && hasTarget) {
						var sourceStatement = sourceEnumerator.Current;
						Debug.Assert(sourceStatement != null);
						var targetStatement = targetEnumerator.Current;
						Debug.Assert(targetStatement != null);
						var diff = string.Compare(sourceStatement.ObjectName, targetStatement.ObjectName, StringComparison.OrdinalIgnoreCase);
						if (diff < 0) {
							yield return new KeyValuePair<IAlterableCreateStatement, InventoryObjectDifference>(sourceStatement, InventoryObjectDifference.SourceOnly);
							hasSource = sourceEnumerator.MoveNext();
						} else if (diff > 0) {
							yield return new KeyValuePair<IAlterableCreateStatement, InventoryObjectDifference>(targetStatement, InventoryObjectDifference.TargetOnly);
							hasTarget = targetEnumerator.MoveNext();
						} else {
							yield return new KeyValuePair<IAlterableCreateStatement, InventoryObjectDifference>(targetStatement, targetStatement.Equals(sourceStatement, engine) ? InventoryObjectDifference.None : InventoryObjectDifference.Different);
							hasSource = sourceEnumerator.MoveNext();
							hasTarget = targetEnumerator.MoveNext();
						}
					}
					while (hasSource) {
						yield return new KeyValuePair<IAlterableCreateStatement, InventoryObjectDifference>(sourceEnumerator.Current, InventoryObjectDifference.SourceOnly);
						hasSource = sourceEnumerator.MoveNext();
					}
					while (hasTarget) {
						yield return new KeyValuePair<IAlterableCreateStatement, InventoryObjectDifference>(targetEnumerator.Current, InventoryObjectDifference.TargetOnly);
						hasTarget = targetEnumerator.MoveNext();
					}
				}
			}
		}

		private static IEnumerable<IAlterableCreateStatement> GetOrderedFragments(Inventory source, DatabaseEngine engine) {
			return source.Objects.SelectMany(s => s.CreateStatementFragments(CreateFragmentMode.Alter)).Where(s => s.DoesApplyToEngine(engine)).OrderBy(s => s.ObjectName, StringComparer.OrdinalIgnoreCase);
		}

		protected static string WriteStatement(IScriptableStatement statement, StringBuilder buffer, DatabaseEngine targetEngine) {
			buffer.Length = 0;
			using (var writer = new StringWriter(buffer)) {
				statement.WriteTo(new SqlWriter(writer, targetEngine));
			}
			return buffer.ToString();
		}

		private readonly HashSet<string> objectSchemas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private readonly SortedDictionary<string, CreateStatement> objects = new SortedDictionary<string, CreateStatement>(StringComparer.OrdinalIgnoreCase);
		private readonly Stack<SchemaName> qualificationStack = new Stack<SchemaName>(4);

		protected Inventory() {
			qualificationStack.Push(null);
		}

		public bool IsEmpty => objects.Count == 0;

		public ICollection<CreateStatement> Objects => objects.Values;

		protected internal string ObjectSchema {
			get {
				var qualification = qualificationStack.Peek();
				return (qualification == null) ? string.Empty : qualification.Value;
			}
		}

		protected HashSet<string> ObjectSchemas => objectSchemas;

		public void Dump(string schemaName, RichTextWriter writer) {
			writer.SetStyle(SqlTextKind.Comment);
			writer.WriteLine("-- Inventory hash: {0}", BitConverter.ToString(GetInventoryHash(DatabaseEngine.Unknown)));
			var sqlWriter = new SqlWriter(writer, DatabaseEngine.Unknown);
			SetQualification(schemaName);
			try {
				foreach (var statement in objects.OrderBy(pair => pair.Key).Select(pair => pair.Value)) {
					writer.WriteLine();
					SetQualification(null);
					try {
						writer.SetStyle(SqlTextKind.Comment);
						writer.WriteLine("-- Object hash: {0}", BitConverter.ToString(statement.GetHash(DatabaseEngine.Unknown)));
					} finally {
						UnsetQualification();
					}
					statement.WriteTo(sqlWriter);
					writer.SetStyle(SqlTextKind.Normal);
					writer.WriteLine(";");
				}
			} finally {
				UnsetQualification();
				writer.Reset();
			}
		}

		public T Find<T>(string objectName) where T: CreateStatement {
			var result = FindInternal<T>(objectName);
			if (result == null) {
				throw new ArgumentException($"The {typeof(T).Name} object [{objectName}] does not exist", nameof(objectName));
			}
			return result;
		}

		public byte[] GetInventoryHash(DatabaseEngine targetEngine) {
			SetQualification(null);
			try {
				using (var writer = new HashWriter()) {
					var sqlWriter = new SqlWriter(writer, targetEngine, SqlWriterMode.ForHashing);
					foreach (var statement in objects.Values) {
						if (statement.DoesApplyToEngine(targetEngine)) {
							statement.WriteTo(sqlWriter);
						}
					}
					var inventoryHash = writer.ToArray();
					for (var i = 0; i < hashXor.Length; i++) {
						inventoryHash[i] ^= hashXor[i];
					}
					return inventoryHash;
				}
			} finally {
				UnsetQualification();
			}
		}

		public bool IsSameInventoryHash(DatabaseEngine targetEngine, byte[] inventoryHash) {
			if (inventoryHash == null) {
				throw new ArgumentNullException(nameof(inventoryHash));
			}
			return HashWriter.HashEqual(GetInventoryHash(targetEngine), inventoryHash);
		}

		public bool TryFind<T>(string objectName, out T result) where T: CreateStatement {
			result = FindInternal<T>(objectName);
			return result != null;
		}

		protected internal void SetQualification(string schemaName) {
			qualificationStack.Push(string.IsNullOrEmpty(schemaName) ? null : new SchemaName(schemaName));
		}

		protected internal void UnsetQualification() {
			qualificationStack.Pop();
		}

		protected virtual void AddObject(CreateStatement createStatement) {
			if (createStatement == null) {
				throw new ArgumentNullException(nameof(createStatement));
			}
			objectSchemas.Add(createStatement.ObjectSchema);
			foreach (var qualifiedName in createStatement.GetObjectSchemaQualifiedNames()) {
				qualifiedName.SetOverride(this);
			}
			createStatement.ComputeHashCode();
			objects.Add(createStatement.ObjectName, createStatement);
		}

		protected IEnumerable<CreateStatement> ProcessSingleScript(TextReader scriptReader, Action<Statement> unsupportedStatementFound) {
			var objects = new List<CreateStatement>();
			CreateTableStatement createTable = null;
			if (log.IsTraceEnabled) {
				var sql = scriptReader.ReadToEnd();
				scriptReader = new StringReader(sql);
				log.Trace("Processing SQL script:\n{sql}", sql);
			}
			foreach (var statement in ScriptParser.Parse(scriptReader)) {
				if (!((statement is SetOptionStatement) || (statement is AlterTableCheckConstraintStatementBase))) {
					if (statement is IApplicableTo<CreateTableStatement> addToTable) {
						if ((createTable == null) || (!createTable.TableName.Name.Equals(addToTable.QualifiedName.Name))) {
							throw DatabaseInventory.CreateException("Statement tries to modify another table:", statement, DatabaseEngine.Unknown);
						}
						addToTable.ApplyTo(createTable);
					} else if (!(statement is CreateStatement createStatement)) {
						unsupportedStatementFound?.Invoke(statement);
					} else {
						if (createStatement is CreateTableStatement createTableStatement) {
							createTable = createTableStatement;
						}
						objects.Add(createStatement);
					}
				}
			}
			foreach (var statement in objects) {
				AddObject(statement);
			}
			return objects;
		}

		private T FindInternal<T>(string objectName) where T: CreateStatement {
			if (objectName == null) {
				throw new ArgumentNullException(nameof(objectName));
			}
			if (objects.TryGetValue(objectName, out var statement)) {
				return statement as T;
			}
			return null;
		}

		SchemaName IQualified<SchemaName>.Qualification => qualificationStack.Peek();
	}
}

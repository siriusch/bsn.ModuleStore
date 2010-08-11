using System;
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CommonTableExpression: SqlToken, IScriptable {
		private readonly AliasName aliasName;
		private readonly List<ColumnName> columnNames;
		private readonly SelectQuery selectQuery;

		[Rule("<CTE> ::= <AliasName> <ColumnNameGroup> AS '(' <SelectQuery> ')'", ConstructorParameterMapping=new[] { 0, 1, 4 })]
		public CommonTableExpression(AliasName aliasName, Optional<Sequence<ColumnName>> columnNames, SelectQuery selectQuery) {
			if (aliasName == null) {
				throw new ArgumentNullException("aliasName");
			}
			if (selectQuery == null) {
				throw new ArgumentNullException("selectQuery");
			}
			this.aliasName = aliasName;
			this.columnNames = columnNames.ToList();
			this.selectQuery = selectQuery;
		}

		public AliasName AliasName {
			get {
				return aliasName;
			}
		}

		public SelectQuery SelectQuery {
			get {
				return selectQuery;
			}
		}

		public List<ColumnName> ColumnNames {
			get {
				return columnNames;
			}
		}

		public void WriteTo(TextWriter writer) {
			writer.WriteScript(aliasName);
			if (columnNames.Count > 0) {
				writer.Write(" (");
				writer.WriteSequence(columnNames, null, ", ", null);
				writer.Write(')');
			}
			writer.WriteLine(" AS (");
			writer.WriteScript(selectQuery);
			writer.WriteLine();
			writer.Write(')');
		}
	}
}
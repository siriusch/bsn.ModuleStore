using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CommonTableExpression: SqlScriptableToken {
		private readonly AliasName aliasName;
		private readonly List<ColumnName> columnNames;
		private readonly SelectQuery selectQuery;

		[Rule("<CTE> ::= <AliasName> <ColumnNameGroup> AS '(' <SelectQuery> ')'", ConstructorParameterMapping = new[] {0, 1, 4})]
		public CommonTableExpression(AliasName aliasName, Optional<Sequence<ColumnName>> columnNames, SelectQuery selectQuery) {
			Debug.Assert(aliasName != null);
			Debug.Assert(selectQuery != null);
			this.aliasName = aliasName;
			this.columnNames = columnNames.ToList();
			this.selectQuery = selectQuery;
		}

		public AliasName AliasName {
			get {
				return aliasName;
			}
		}

		public IEnumerable<ColumnName> ColumnNames {
			get {
				return columnNames;
			}
		}

		public SelectQuery SelectQuery {
			get {
				return selectQuery;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(aliasName, WhitespacePadding.None);
			if (columnNames.Count > 0) {
				writer.Write(" (");
				writer.WriteScriptSequence(columnNames, WhitespacePadding.None, ", ");
				writer.Write(')');
			}
			writer.Write(" AS (");
			writer.IncreaseIndent();
			writer.WriteScript(selectQuery, WhitespacePadding.NewlineBefore);
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(')');
		}
	}
}
using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateUpdate: Predicate {
		private readonly ColumnName columnName;

		[Rule("<PredicateFunction> ::= ~UPDATE ~'(' <ColumnName> ~')'")]
		public PredicateUpdate(ColumnName columnName) {
			this.columnName = columnName;
			Debug.Assert(columnName != null);
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("UPDATE(");
			writer.WriteScript(columnName, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}
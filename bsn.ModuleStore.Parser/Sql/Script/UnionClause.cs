using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class UnionClause: SqlScriptableToken {
		private readonly SelectQuery selectQuery;

		[Rule("<UnionClause> ::= ~UNION <SelectQuery>")]
		public UnionClause(SelectQuery selectQuery): base() {
			Debug.Assert(selectQuery != null);
			this.selectQuery = selectQuery;
		}

		public virtual bool All {
			get {
				return false;
			}
		}

		public SelectQuery SelectQuery {
			get {
				return selectQuery;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("UNION ");
			if (All) {
				writer.Write("ALL ");
			}
			writer.WriteScript(selectQuery, WhitespacePadding.NewlineBefore);
		}
	}
}
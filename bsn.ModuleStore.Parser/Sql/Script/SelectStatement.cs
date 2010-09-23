using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[CommonTableExpressionScope]
	public sealed class SelectStatement: Statement {
		private readonly List<CommonTableExpression> ctes;
		private readonly QueryHint queryHint;
		private readonly SelectQuery selectQuery;

		[Rule("<SelectStatement> ::= <SelectQuery> <QueryHint>")]
		public SelectStatement(SelectQuery selectQuery, QueryHint queryHint): this(null, selectQuery, queryHint) {}

		[Rule("<SelectStatement> ::= ~WITH <CTEList> <SelectQuery> <QueryHint>")]
		public SelectStatement(Sequence<CommonTableExpression> ctes, SelectQuery selectQuery, QueryHint queryHint) {
			Debug.Assert(selectQuery != null);
			this.ctes = ctes.ToList();
			this.selectQuery = selectQuery;
			this.queryHint = queryHint;
		}

		public IEnumerable<CommonTableExpression> Ctes {
			get {
				return ctes;
			}
		}

		public QueryHint QueryHint {
			get {
				return queryHint;
			}
		}

		public SelectQuery SelectQuery {
			get {
				return selectQuery;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteCommonTableExpressions(ctes);
			writer.IncreaseIndent();
			writer.WriteScript(selectQuery, WhitespacePadding.None);
			writer.WriteScript(queryHint, WhitespacePadding.SpaceBefore);
			writer.DecreaseIndent();
		}
	}
}
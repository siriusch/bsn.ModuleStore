using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[CommonTableExpressionScope]
	public sealed class SelectStatement: Statement {
		private readonly QueryHint queryHint;
		private readonly QueryOptions queryOptions;
		private readonly SelectQuery selectQuery;

		[Rule("<SelectStatement> ::= <QueryOptions> <SelectQuery> <QueryHint>")]
		public SelectStatement(QueryOptions queryOptions, SelectQuery selectQuery, QueryHint queryHint) {
			Debug.Assert(selectQuery != null);
			this.queryOptions = queryOptions;
			this.selectQuery = selectQuery;
			this.queryHint = queryHint;
		}

		public QueryHint QueryHint {
			get {
				return queryHint;
			}
		}

		public QueryOptions QueryOptions {
			get {
				return queryOptions;
			}
		}

		public SelectQuery SelectQuery {
			get {
				return selectQuery;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteScript(queryOptions, WhitespacePadding.NewlineAfter);
			writer.IncreaseIndent();
			writer.WriteScript(selectQuery, WhitespacePadding.None);
			writer.WriteScript(queryHint, WhitespacePadding.SpaceBefore);
			writer.DecreaseIndent();
		}
	}
}
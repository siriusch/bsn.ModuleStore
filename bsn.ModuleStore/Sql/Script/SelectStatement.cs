using System;
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SelectStatement: Statement {
		private readonly List<CommonTableExpression> ctes;
		private readonly ForClause forClause;
		private readonly QueryHint queryHint;
		private readonly SelectQuery selectQuery;

		[Rule("<SelectStatement> ::= <CTEGroup> <SelectQuery> <ForClause> <QueryHint>")]
		public SelectStatement(Optional<Sequence<CommonTableExpression>> ctes, SelectQuery selectQuery, ForClause forClause, QueryHint queryHint) {
			if (selectQuery == null) {
				throw new ArgumentNullException("selectQuery");
			}
			this.ctes = ctes.ToList();
			this.selectQuery = selectQuery;
			this.forClause = forClause;
			this.queryHint = queryHint;
		}

		public List<CommonTableExpression> Ctes {
			get {
				return ctes;
			}
		}

		public ForClause ForClause {
			get {
				return forClause;
			}
		}

		public SelectQuery SelectQuery {
			get {
				return selectQuery;
			}
		}

		public QueryHint QueryHint {
			get {
				return queryHint;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteCommonTableExpressions(ctes);
			writer.WriteScript(selectQuery);
			writer.WriteScript(forClause, " ", null);
			writer.WriteScript(queryHint, " ", null);
		}
	}
}
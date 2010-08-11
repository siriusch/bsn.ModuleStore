using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SelectStatement: Statement {
		private readonly List<CommonTableExpression> ctes;
		private readonly ForClause forClause;
		private readonly QueryHint queryHint;
		private readonly SelectQuery selectQuery;

		[Rule("<SelectStatement> ::= <SelectQuery> <ForClause> <QueryHint>")]
		public SelectStatement(SelectQuery selectQuery, ForClause forClause, QueryHint queryHint): this(null, selectQuery, forClause, queryHint) {}

		[Rule("<SelectStatement> ::= WITH <CTEList> <SelectQuery> <ForClause> <QueryHint>", ConstructorParameterMapping = new[] {1, 2, 3, 4})]
		public SelectStatement(Sequence<CommonTableExpression> ctes, SelectQuery selectQuery, ForClause forClause, QueryHint queryHint) {
			Debug.Assert(selectQuery != null);
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

		public override void WriteTo(SqlWriter writer) {
			writer.WriteCommonTableExpressions(ctes);
			writer.WriteScript(selectQuery);
			writer.WriteScript(forClause, " ", null);
			writer.WriteScript(queryHint, " ", null);
		}
	}
}
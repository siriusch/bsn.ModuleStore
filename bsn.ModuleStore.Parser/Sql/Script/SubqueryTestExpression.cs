using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class SubqueryTestExpression: Expression {
		private readonly bool any;
		private readonly SelectQuery subquery;

		[Rule("<SubqueryTest> ::= ANY ~'(' <SelectQuery> ~')'")]
		[Rule("<SubqueryTest> ::= ALL ~'(' <SelectQuery> ~')'")]
		[Rule("<SubqueryTest> ::= SOME ~'(' <SelectQuery> ~')'")]
		public SubqueryTestExpression(DuplicateRestrictionToken restriction, SelectQuery subquery) {
			this.subquery = subquery;
			any = restriction.Distinct;
		}

		public bool Any {
			get {
				return any;
			}
		}

		public SelectQuery Subquery {
			get {
				return subquery;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteKeyword(any ? "ANY" : "ALL");
			writer.Write(" (");
			using (writer.Indent()) {
				writer.WriteScript(subquery, WhitespacePadding.None);
			}
			writer.Write(')');
		}
	}
}

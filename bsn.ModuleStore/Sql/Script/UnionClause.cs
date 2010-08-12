using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class UnionClause: SqlScriptableToken, IOptional {
		private readonly SelectQuery selectQuery;

		[Rule("<UnionClause> ::=")]
		public UnionClause(): this(null) {}

		[Rule("<UnionClause> ::= UNION <SelectQuery>", ConstructorParameterMapping = new[] {1})]
		public UnionClause(SelectQuery selectQuery): base() {
			this.selectQuery = selectQuery;
		}

		public virtual bool All {
			get {
				return false;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			if (HasValue) {
				writer.Write("UNION ");
				if (All) {
					writer.Write("ALL ");
				}
				writer.WriteScript(selectQuery, WhitespacePadding.NewlineBefore);
			}
		}

		public bool HasValue {
			get {
				return selectQuery != null;
			}
		}
	}
}
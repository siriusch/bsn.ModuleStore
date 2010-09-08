using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ForClause: SqlScriptableToken, IOptional {
		[Rule("<ForClause> ::=")]
		public ForClause() {}

		public virtual SelectFor SelectFor {
			get {
				return SelectFor.None;
			}
		}

		public override void WriteTo(SqlWriter writer) {}

		public bool HasValue {
			get {
				return SelectFor != SelectFor.None;
			}
		}
	}
}
using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class QueryHint: SqlScriptableToken, IOptional {
		[Rule("<QueryHint> ::=")]
		public QueryHint() {}

		public override void WriteTo(SqlWriter writer) {}

		public virtual bool HasValue {
			get {
				return false;
			}
		}
	}
}
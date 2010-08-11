using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ForClause: SqlToken, IScriptable, IOptional {
		[Rule("<ForClause> ::=")]
		public ForClause() {}

		public virtual SelectFor SelectFor {
			get {
				return SelectFor.None;
			}
		}

		public bool HasValue {
			get {
				return SelectFor != SelectFor.None;
			}
		}

		public virtual void WriteTo(SqlWriter writer) {}
	}
}
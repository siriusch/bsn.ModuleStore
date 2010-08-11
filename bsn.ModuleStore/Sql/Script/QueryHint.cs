using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class QueryHint: SqlToken, IScriptable, IOptional {
		[Rule("<QueryHint> ::=")]
		public QueryHint() {}

		public virtual void WriteTo(SqlWriter writer) {}

		public virtual bool HasValue {
			get {
				return false;
			}
		}
	}
}

using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class RowsetAlias: SqlScriptableToken, IOptional {
		[Rule("<RowsetAlias> ::=")]
		public RowsetAlias() {}

		public override void WriteTo(SqlWriter writer) {}

		public virtual bool HasValue {
			get {
				return false;
			}
		}
	}
}
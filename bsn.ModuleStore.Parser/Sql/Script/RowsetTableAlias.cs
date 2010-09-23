using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class RowsetTableAlias: RowsetAlias {
		private readonly AliasName aliasName;

		[Rule("<RowsetAlias> ::= ~<OptionalAs> <AliasName>")]
		public RowsetTableAlias(AliasName aliasName) {
			Debug.Assert(aliasName != null);
			this.aliasName = aliasName;
		}

		public AliasName AliasName {
			get {
				return aliasName;
			}
		}

		public override sealed bool HasValue {
			get {
				return true;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("AS ");
			writer.WriteScript(aliasName, WhitespacePadding.None);
		}
	}
}
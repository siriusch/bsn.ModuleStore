using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("FOR_BROWSE")]
	public class ForBrowsingClause: ForClause {
		public ForBrowsingClause() {}

		public override SelectFor SelectFor {
			get {
				return SelectFor.Browse;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("FOR BROWSE");
		}
	}
}
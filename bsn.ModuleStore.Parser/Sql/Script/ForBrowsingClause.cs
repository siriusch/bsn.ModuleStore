using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ForBrowsingClause: ForClause {
		[Rule("<ForClause> ::= FOR BROWSE", AllowTruncationForConstructor = true)]
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
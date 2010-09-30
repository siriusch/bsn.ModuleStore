using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SetReadUncommitted: SetValue {
		[Rule("<SetValue> ::= ~READ ~UNCOMMITTED")]
		public SetReadUncommitted() {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("READ UNCOMMITTED");
		}
	}
}
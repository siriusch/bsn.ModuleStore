using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SetReadCommitted: SetValue {
		[Rule("<SetValue> ::= ~READ ~COMMITTED")]
		public SetReadCommitted() {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("READ COMMITTED");
		}
	}
}
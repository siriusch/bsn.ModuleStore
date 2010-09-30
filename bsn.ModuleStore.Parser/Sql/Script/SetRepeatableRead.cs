using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SetRepeatableRead: SetValue {
		[Rule("<SetValue> ::= ~REPEATABLE ~READ")]
		public SetRepeatableRead() {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("REPEATABLE READ");
		}
	}
}
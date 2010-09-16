using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ForeignKeySetDefaultAction: ForeignKeyAction {
		[Rule("<ForeignKeyAction> ::= ~ON DELETE ~SET ~DEFAULT")]
		[Rule("<ForeignKeyAction> ::= ~ON UPDATE ~SET ~DEFAULT")]
		public ForeignKeySetDefaultAction(DmlOperationToken operation): base(operation) {}

		public override ForeignKeyActionKind Kind {
			get {
				return ForeignKeyActionKind.SetDefault;
			}
		}

		protected override string ActionString {
			get {
				return "SET DEFAULT";
			}
		}
	}
}
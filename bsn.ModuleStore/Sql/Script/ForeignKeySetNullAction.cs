using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ForeignKeySetNullAction: ForeignKeyAction {
		[Rule("<ForeignKeyAction> ::= ON DELETE SET NULL", ConstructorParameterMapping = new[] {1})]
		[Rule("<ForeignKeyAction> ::= ON UPDATE SET NULL", ConstructorParameterMapping = new[] {1})]
		public ForeignKeySetNullAction(DmlOperationToken operation): base(operation) {}

		public override ForeignKeyActionKind Kind {
			get {
				return ForeignKeyActionKind.SetNull;
			}
		}

		protected override string ActionString {
			get {
				return "SET NULL";
			}
		}
	}
}
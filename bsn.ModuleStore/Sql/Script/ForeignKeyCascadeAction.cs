using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ForeignKeyCascadeAction: ForeignKeyAction {
		[Rule("<ForeignKeyAction> ::= ON DELETE CASCADE", ConstructorParameterMapping = new[] {1})]
		[Rule("<ForeignKeyAction> ::= ON UPDATE CASCADE", ConstructorParameterMapping = new[] {1})]
		public ForeignKeyCascadeAction(DmlOperationToken operation): base(operation) {}

		public override ForeignKeyActionKind Kind {
			get {
				return ForeignKeyActionKind.Cascade;
			}
		}

		protected override string ActionString {
			get {
				return "CASCADE";
			}
		}
	}
}
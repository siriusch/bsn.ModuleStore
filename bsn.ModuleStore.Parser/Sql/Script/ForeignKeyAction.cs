using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class ForeignKeyAction: SqlScriptableToken {
		private readonly DmlOperation operation;

		[Rule("<ForeignKeyAction> ::= ON DELETE NO_ACTION", ConstructorParameterMapping = new[] {1})]
		[Rule("<ForeignKeyAction> ::= ON UPDATE NO_ACTION", ConstructorParameterMapping = new[] {1})]
		public ForeignKeyAction(DmlOperationToken operation): base() {
			Debug.Assert(operation != null);
			this.operation = operation.Operation;
		}

		public virtual ForeignKeyActionKind Kind {
			get {
				return ForeignKeyActionKind.None;
			}
		}

		protected virtual string ActionString {
			get {
				return "NO ACTION";
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("ON ");
			writer.WriteEnum(operation, WhitespacePadding.None);
			writer.Write(' ');
			writer.Write(ActionString);
		}
	}
}
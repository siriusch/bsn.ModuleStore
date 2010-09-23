using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ConstraintDefaultFunctionValue: Expression {
		private readonly FunctionCall function;

		[Rule("<ConstraintDefaultValue> ::= <FunctionCall>")]
		public ConstraintDefaultFunctionValue(FunctionCall function) {
			Debug.Assert(function != null);
			this.function = function;
		}

		public FunctionCall Function {
			get {
				return function;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteCommentsTo(writer);
			writer.WriteScript(function, WhitespacePadding.None);
		}
	}
}
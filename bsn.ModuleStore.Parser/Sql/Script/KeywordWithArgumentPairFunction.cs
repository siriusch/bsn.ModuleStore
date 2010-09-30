using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class KeywordWithArgumentPairFunction: KeywordFunction {
		private readonly Expression firstExpression;
		private readonly Expression secondExpression;

		[Rule("<FunctionCall> ::= LEFT ~'(' <Expression> ~',' <Expression> ~')'")]
		[Rule("<FunctionCall> ::= RIGHT ~'(' <Expression> ~',' <Expression> ~')'")]
		[Rule("<FunctionCall> ::= NULLIF ~'(' <Expression> ~',' <Expression> ~')'")]
		public KeywordWithArgumentPairFunction(ReservedKeyword keyword, Expression firstExpression, Expression secondExpression): base(keyword) {
			Debug.Assert(firstExpression != null);
			Debug.Assert(secondExpression != null);
			this.firstExpression = firstExpression;
			this.secondExpression = secondExpression;
		}

		public Expression FirstExpression {
			get {
				return firstExpression;
			}
		}

		public Expression SecondExpression {
			get {
				return secondExpression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write('(');
			writer.WriteScript(firstExpression, WhitespacePadding.None);
			writer.Write(", ");
			writer.WriteScript(secondExpression, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}
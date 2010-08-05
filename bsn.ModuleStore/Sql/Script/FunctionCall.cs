using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class FunctionCall: SqlToken {
		private readonly Sequence<Expression> arguments;
		private readonly FunctionName functionName;

		[Rule("<FunctionCall> ::= <FunctionName> '(' ')'", AllowTruncationForConstructor = true)]
		public FunctionCall(FunctionName functionName): this(functionName, null) {}

		[Rule("<FunctionCall> ::= <FunctionName> '(' <ExpressionList> ')'", ConstructorParameterMapping = new[] {0, 2})]
		public FunctionCall(FunctionName functionName, Sequence<Expression> arguments) {
			if (functionName == null) {
				throw new ArgumentNullException("functionName");
			}
			this.functionName = functionName;
			this.arguments = arguments;
		}
	}
}
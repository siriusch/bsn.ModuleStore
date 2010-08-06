using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExpressionFunctionCall: ExpressionFunction {
		private readonly Sequence<Expression> arguments;
		private readonly FunctionName functionName;

		[Rule("<FunctionCall> ::= <FunctionName> '(' ')'", AllowTruncationForConstructor = true)]
		public ExpressionFunctionCall(FunctionName functionName): this(functionName, null) {}

		[Rule("<FunctionCall> ::= <FunctionName> '(' <ExpressionList> ')'", ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<ExpressionFunction> ::= COALESCE '(' <ExpressionList> ')'", ConstructorParameterMapping = new[] {0, 2})]
		public ExpressionFunctionCall(FunctionName functionName, Sequence<Expression> arguments) {
			if (functionName == null) {
				throw new ArgumentNullException("functionName");
			}
			this.functionName = functionName;
			this.arguments = arguments;
		}

		public Sequence<Expression> Arguments {
			get {
				return arguments;
			}
		}

		public FunctionName FunctionName {
			get {
				return functionName;
			}
		}
	}
}
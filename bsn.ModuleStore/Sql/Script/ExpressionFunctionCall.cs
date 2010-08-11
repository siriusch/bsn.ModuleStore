using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionFunctionCall: ExpressionFunction {
		private readonly List<Expression> arguments;
		private readonly FunctionName functionName;

		[Rule("<FunctionCall> ::= <FunctionName> '(' ')'", AllowTruncationForConstructor = true)]
		public ExpressionFunctionCall(FunctionName functionName): this(functionName, null) {}

		[Rule("<FunctionCall> ::= <FunctionName> '(' <ExpressionList> ')'", ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<ExpressionFunction> ::= COALESCE '(' <ExpressionList> ')'", ConstructorParameterMapping = new[] {0, 2})]
		public ExpressionFunctionCall(FunctionName functionName, Sequence<Expression> arguments) {
			Debug.Assert(functionName != null);
			this.functionName = functionName;
			this.arguments = arguments.ToList();
		}

		public List<Expression> Arguments {
			get {
				return arguments;
			}
		}

		public FunctionName FunctionName {
			get {
				return functionName;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteScript(functionName);
			writer.Write('(');
			writer.WriteSequence(arguments, null, ", ", null);
			writer.Write(')');
		}
	}
}
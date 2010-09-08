using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionFunctionCall: ExpressionFunction {
		private static Qualified<SchemaName, FunctionName> CreateFunctionName(TableName qualification, ExpressionFunctionCall call) {
			Qualified<SchemaName, FunctionName> result = new Qualified<SchemaName, FunctionName>(new SchemaName(qualification.Value), call.functionName.Name);
			result.SetPosition(((IToken)qualification).Position);
			return result;
		}

		private readonly List<Expression> arguments;
		private readonly Qualified<SchemaName, FunctionName> functionName;

		[Rule("<FunctionCall> ::= <FunctionName> '(' ')'", AllowTruncationForConstructor = true)]
		public ExpressionFunctionCall(FunctionName functionName): this(functionName, null) {}

		[Rule("<FunctionCall> ::= <FunctionName> '(' <ExpressionList> ')'", ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<Value> ::= COALESCE '(' <ExpressionList> ')'", ConstructorParameterMapping = new[] {0, 2})]
		public ExpressionFunctionCall(FunctionName functionName, Sequence<Expression> arguments): this(new Qualified<SchemaName, FunctionName>(functionName), arguments.ToList()) {}

		[Rule("<Value> ::= <TableName> '.' <FunctionCall>", ConstructorParameterMapping = new[] {0, 2})]
		public ExpressionFunctionCall(TableName qualification, ExpressionFunctionCall call): this(CreateFunctionName(qualification, call), call.arguments) {}

		private ExpressionFunctionCall(Qualified<SchemaName, FunctionName> functionName, List<Expression> arguments) {
			Debug.Assert(functionName != null);
			this.functionName = functionName;
			this.arguments = arguments;
		}

		public IEnumerable<Expression> Arguments {
			get {
				return arguments;
			}
		}

		public Qualified<SchemaName, FunctionName> FunctionName {
			get {
				return functionName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(functionName, WhitespacePadding.None);
			writer.Write('(');
			writer.WriteScriptSequence(arguments, WhitespacePadding.None, ", ");
			writer.Write(')');
		}
	}
}
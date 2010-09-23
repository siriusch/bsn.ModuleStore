using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class NamedFunction: FunctionCall {
		private readonly List<Expression> arguments;
		private readonly Qualified<SchemaName, FunctionName> functionName;

		[Rule("<NamedFunction> ::= <FunctionName> ~'(' ~')'")]
		public NamedFunction(FunctionName functionName): this(functionName, null) {}

		[Rule("<NamedFunction> ::= <FunctionName> ~'(' <ExpressionList> ~')'")]
		[Rule("<FunctionCall> ::= COALESCE ~'(' <ExpressionList> ~')'")]
		public NamedFunction(FunctionName functionName, Sequence<Expression> arguments): this(new Qualified<SchemaName, FunctionName>(functionName), arguments.ToList()) {}

		private NamedFunction(Qualified<SchemaName, FunctionName> functionName, List<Expression> arguments) {
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

		internal NamedFunction QualifiedWith(SqlName qualification) {
			Qualified<SchemaName, FunctionName> qualifiedFunctionName = new Qualified<SchemaName, FunctionName>(new SchemaName(qualification.Value), functionName.Name);
			qualifiedFunctionName.SetPosition(((IToken)qualification).Position);
			return new NamedFunction(qualifiedFunctionName, arguments);
		}
	}
}
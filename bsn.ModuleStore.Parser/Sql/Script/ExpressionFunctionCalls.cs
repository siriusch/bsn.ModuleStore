using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExpressionFunctionCalls: ExpressionFunction {
		private readonly FunctionCall function;
		private readonly List<NamedFunction> functions;

		// the "schema-qualified function" trick - avoids reduce-reduce issue
		[Rule("<Value> ::= <TableName> ~'.' <NamedFunctionList>")]
		public ExpressionFunctionCalls(TableName schemaName, Sequence<NamedFunction> functions): this(functions.Item.QualifiedWith(schemaName), functions.Next) {}

		[Rule("<Value> ::= <FunctionCall>")]
		public ExpressionFunctionCalls(FunctionCall function): this(function, null) {}

		[Rule("<Value> ::= <FunctionCall> ~'.' <NamedFunctionList>")]
		public ExpressionFunctionCalls(FunctionCall function, Sequence<NamedFunction> functions) {
			Debug.Assert(function != null);
			this.function = function;
			this.functions = functions.ToList();
		}

		public FunctionCall Function {
			get {
				return function;
			}
		}

		public IEnumerable<NamedFunction> Functions {
			get {
				return functions;
			}
		}

		public override sealed void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			WriteToInternal(writer);
		}

		protected virtual void WriteToInternal(SqlWriter writer) {
			writer.WriteScript(function, WhitespacePadding.None);
			if (functions.Count > 0) {
				writer.Write('.');
				writer.WriteScriptSequence(functions, WhitespacePadding.None, ".");
			}
		}
	}
}
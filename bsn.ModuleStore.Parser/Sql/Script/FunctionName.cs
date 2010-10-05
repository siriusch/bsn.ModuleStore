using System;
using System.Collections.Generic;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("COALESCE")]
	[Terminal("CONVERT")]
	public sealed class FunctionName: SqlQuotedName {
		private static KeyValuePair<string, bool> FormatName(string name) {
			bool isBuiltIn = ScriptParser.TryGetBuiltinFunctionName(ref name);
			return new KeyValuePair<string, bool>(name, isBuiltIn);
		}

		private readonly bool builtinFunction;

		public FunctionName(string name): this(FormatName(name)) {}

		[Rule("<FunctionName> ::= SystemFuncId")]
		public FunctionName(SysFunctionIdentifier identifier): this(new KeyValuePair<string, bool>(identifier.Value, true)) {}

		[Rule("<FunctionName> ::= Id")]
		[Rule("<FunctionName> ::= QuotedId")]
		public FunctionName(Identifier identifier): this(identifier.Value) {}

		private FunctionName(KeyValuePair<string, bool> functionName): base(functionName.Key) {
			builtinFunction = functionName.Value;
		}

		public bool IsBuiltinFunction {
			get {
				return builtinFunction;
			}
		}

		protected internal override void WriteToInternal(SqlWriter writer, bool isPartOfQualifiedName) {
			if (IsBuiltinFunction) {
				writer.Write(Value);
			} else {
				base.WriteToInternal(writer, true);
			}
		}
	}
}
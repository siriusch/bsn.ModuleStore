using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("COALESCE")]
	[Terminal("CONVERT")]
	public sealed class FunctionName: SqlQuotedName {
		private static string FormatName(string name) {
			ScriptParser.TryGetBuiltinFunctionName(ref name);
			return name;
		}

		private readonly bool builtinFunction;

		public FunctionName(string name): base(FormatName(name)) {
			builtinFunction = ScriptParser.IsBuiltinFunctionName(name);
		}

		[Rule("<FunctionName> ::= SystemFuncId")]
		public FunctionName(SysFunctionIdentifier identifier): base(identifier.Value) {
			builtinFunction = true;
		}

		[Rule("<FunctionName> ::= Id")]
		[Rule("<FunctionName> ::= QuotedId")]
		public FunctionName(Identifier identifier): this(identifier.Value) {}

		public bool IsBuiltinFunction {
			get {
				return builtinFunction;
			}
		}

		protected internal override void WriteToInternal(SqlWriter writer, bool isPartOfQualifiedName) {
			if (isPartOfQualifiedName || (!IsBuiltinFunction)) {
				base.WriteToInternal(writer, true);
			} else {
				writer.Write(Value);
			}
		}
	}
}
using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("COALESCE")]
	[Terminal("CONVERT")]
	[Terminal("NULLIF")]
#warning check if there are more reserved words which are in fact functions
	public sealed class FunctionName: SqlQuotedName {
		private readonly bool builtinFunction;

		public FunctionName(string name): base(FormatName(name)) {
			builtinFunction = ScriptParser.IsBuiltinFunctionName(name);
		}

		private static string FormatName(string name) {
			ScriptParser.TryGetBuiltinFunctionName(ref name);
			return name;
		}

		[Rule("<FunctionName> ::= SystemFuncId")]
		public FunctionName(SysFunctionIdentifier identifier)
			: base(identifier.Value) {
			builtinFunction = true;
		}

		[Rule("<FunctionName> ::= Id")]
		public FunctionName(SqlIdentifier identifier): this(identifier.Value) {}

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
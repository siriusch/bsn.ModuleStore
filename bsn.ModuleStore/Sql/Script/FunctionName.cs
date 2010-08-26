using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("COALESCE")]
	[Terminal("CONVERT")]
	[Terminal("NULLIF")]
	public sealed class FunctionName: SqlQuotedName {
		private readonly bool systemFunction;

		public FunctionName(string name): base(name.ToUpperInvariant()) {}

		[Rule("<FunctionName> ::= Id")]
		[Rule("<FunctionName> ::= SystemFuncId")]
		public FunctionName(SqlIdentifier identifier): base(identifier.Value) {
			systemFunction = identifier is SysFunctionIdentifier;
		}

		public bool IsBuiltinFunction {
			get {
				return systemFunction || ScriptParser.IsBuiltInFunctionName(Value);
			}
		}

		public bool IsSystemFunction {
			get {
				return systemFunction;
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
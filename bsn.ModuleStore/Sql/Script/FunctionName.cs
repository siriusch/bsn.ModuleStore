using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("COALESCE")]
	public sealed class FunctionName: SqlQuotedName {
		private readonly bool systemFunction;

		public FunctionName(string name): base(name) {}

		[Rule("<FunctionName> ::= Id")]
		[Rule("<FunctionName> ::= SystemFuncId")]
		public FunctionName(SqlIdentifier identifier): base(identifier.Value) {
			systemFunction = identifier is SysFunctionIdentifier;
		}

		public bool IsSystemFunction {
			get {
				return systemFunction;
			}
		}

		public override void WriteTo(TextWriter writer) {
			if (systemFunction) {
				writer.Write(Value);
			} else {
				base.WriteTo(writer);
			}
		}
	}
}
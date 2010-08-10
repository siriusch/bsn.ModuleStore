using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceFunctionRowset: SourceRowset {
		private readonly ExpressionFunctionCall function;

		[Rule("<SourceRowset> ::= <FunctionCall> <OptionalAlias>")]
		public SourceFunctionRowset(ExpressionFunctionCall function, Optional<AliasName> aliasName): base(aliasName) {
			if (function == null) {
				throw new ArgumentNullException("function");
			}
			this.function = function;
		}

		public ExpressionFunctionCall Function {
			get {
				return function;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteScript(function);
			base.WriteTo(writer);
		}
	}
}
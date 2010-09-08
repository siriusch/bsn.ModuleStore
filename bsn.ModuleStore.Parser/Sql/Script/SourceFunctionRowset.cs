using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceFunctionRowset: SourceRowset {
		private readonly ExpressionFunctionCall function;

		[Rule("<SourceRowset> ::= <SchemaName> '.' <FunctionCall> <OptionalAlias>", ConstructorParameterMapping = new[] {0, 2, 3})]
		public SourceFunctionRowset(SchemaName schemaName, ExpressionFunctionCall function, Optional<AliasName> aliasName): this(function, aliasName) {
			function.FunctionName.Qualification = schemaName;
		}

		[Rule("<SourceRowset> ::= <FunctionCall> <OptionalAlias>")]
		public SourceFunctionRowset(ExpressionFunctionCall function, Optional<AliasName> aliasName): base(aliasName) {
			Debug.Assert(function != null);
			this.function = function;
		}

		public ExpressionFunctionCall Function {
			get {
				return function;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(function, WhitespacePadding.None);
			base.WriteTo(writer);
		}
	}
}
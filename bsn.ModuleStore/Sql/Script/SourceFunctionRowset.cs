using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class SourceFunctionRowset: SourceRowset {
		[Rule("<SourceRowset> ::= <FunctionCall> <OptionalAlias>")]
		public SourceFunctionRowset(ExpressionFunctionCall function, Optional<AliasName> aliasName): base(aliasName) {}
	}
}
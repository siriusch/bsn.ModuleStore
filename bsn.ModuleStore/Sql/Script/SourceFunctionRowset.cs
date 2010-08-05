using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class SourceFunctionRowset: SourceRowset {
		[Rule("<SourceRowset> ::= <FunctionCall> <OptionalAlias>")]
		public SourceFunctionRowset(FunctionCall function, Optional<AliasName> aliasName): base(aliasName) {}
	}
}
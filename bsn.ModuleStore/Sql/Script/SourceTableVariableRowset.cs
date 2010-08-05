using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class SourceTableVariableRowset: SourceRowset {
		[Rule("<SourceRowset> ::= <VariableName> <OptionalAlias>")]
		public SourceTableVariableRowset(VariableName tableName, Optional<AliasName> aliasName): base(aliasName) {}
	}
}
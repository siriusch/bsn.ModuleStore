using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class SourceOpenxmlRowset: SourceRowset {
		[Rule("<SourceRowset> ::= <Openxml> <OptionalAlias>")]
		public SourceOpenxmlRowset(OpenxmlFunction openXml, Optional<AliasName> aliasName): base(aliasName) {}
	}
}
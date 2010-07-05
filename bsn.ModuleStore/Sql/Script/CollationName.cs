using System;
using System.Linq;

using bsn.GoldParser.Semantic;

[assembly: RuleTrim("<OptionalCollate> ::= COLLATE <CollationName>", "<CollationName>")]

namespace bsn.ModuleStore.Sql.Script {
	public class CollationName: SqlName {
		[Rule("<OptionalCollate> ::=")]
		public CollationName(): base(null) {}

		[Rule("<CollationName> ::= Id")]
		public CollationName(Identifier identifier): base(identifier.Value) {}
	}
}
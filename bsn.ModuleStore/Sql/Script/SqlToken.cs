using System;
using System.Linq;

using bsn.GoldParser.Semantic;

[assembly: RuleTrim("<OptionalCollate> ::= COLLATE <CollationName>", "<CollationName>")]

namespace bsn.ModuleStore.Sql.Script {
	public class SqlToken: SemanticToken {}
}
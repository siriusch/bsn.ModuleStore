using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class IndexUsing: SqlToken {
		[Rule("<IndexUsing> ::=")]
		public IndexUsing() {}

		[Rule("<IndexUsing> ::= USING_XML_INDEX <IndexName> <IndexFor>", ConstructorParameterMapping = new[] {1, 2})]
		public IndexUsing(IndexName indexName, SqlToken forToken) {}
	}
}
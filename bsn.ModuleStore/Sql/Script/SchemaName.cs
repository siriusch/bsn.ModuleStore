using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class SchemaName: SqlName {
		[Rule("<SchemaName> ::= Id")]
		public SchemaName(Identifier identifier): base(identifier.Value) {}
	}
}
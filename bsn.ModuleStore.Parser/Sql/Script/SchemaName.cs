using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SchemaName: SqlQuotedName {
		[Rule("<SchemaName> ::= Id")]
		[Rule("<SchemaName> ::= QuotedId")]
		public SchemaName(Identifier identifier): this(identifier.Value) {}

		internal SchemaName(string name): base(name) {}
	}
}
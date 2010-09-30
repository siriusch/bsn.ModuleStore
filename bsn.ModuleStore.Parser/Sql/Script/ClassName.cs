using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ClassName: SqlQuotedName {
		[Rule("<ClassName> ::= Id")]
		[Rule("<ClassName> ::= QuotedId")]
		public ClassName(Identifier identifier): this(identifier.Value) {}

		internal ClassName(string name): base(name) {}
	}
}
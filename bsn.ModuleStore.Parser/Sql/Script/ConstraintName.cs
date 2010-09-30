using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ConstraintName: SqlQuotedName {
		[Rule("<ConstraintName> ::= Id")]
		[Rule("<ConstraintName> ::= QuotedId")]
		public ConstraintName(Identifier identifier): base(identifier.Value) {}
	}
}
using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ConstraintName: SqlName {
		[Rule("<ConstraintName> ::= Id")]
		public ConstraintName(Identifier identifier): base(identifier.Value) {}
	}
}
using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class VariableName: SqlName {
		[Rule("<SystemVariableName> ::= SystemVarId")]
		[Rule("<VariableName> ::= LocalId")]
		public VariableName(SqlIdentifier identifier): base(identifier.Value) {}
	}
}
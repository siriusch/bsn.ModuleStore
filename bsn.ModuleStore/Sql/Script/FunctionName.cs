using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class FunctionName: SqlName {
		[Rule("<FunctionName> ::= Id")]
		[Rule("<FunctionName> ::= SystemFuncId")]
		public FunctionName(SqlIdentifier identifier): base(identifier.Value) {}
	}
}
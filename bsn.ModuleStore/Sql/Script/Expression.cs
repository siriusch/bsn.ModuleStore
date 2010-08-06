using System;

using bsn.GoldParser.Semantic;

[assembly: RuleTrim("<ExpressionNegate> ::= '+' <ExpressionFunction>", "<ExpressionFunction>")]

namespace bsn.ModuleStore.Sql.Script {
	public abstract class Expression: SqlComputable {}
}
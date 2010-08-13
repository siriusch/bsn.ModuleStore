using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script;

[assembly: RuleTrim("<ExpressionNegate> ::= '+' <ExpressionCase>", "<ExpressionCase>", SemanticTokenType = typeof(SqlToken))]

namespace bsn.ModuleStore.Sql.Script {
	public abstract class Expression: SqlComputable {}
}
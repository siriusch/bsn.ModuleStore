using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script;

[assembly: RuleTrim("<ExpressionNegate> ::= '+' <ExpressionFunction>", "<ExpressionFunction>", SemanticTokenType=typeof(SqlToken))]

namespace bsn.ModuleStore.Sql.Script {
	public abstract class Expression: SqlComputable {}
}
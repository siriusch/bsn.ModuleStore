using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExpressionRankingFunction: ExpressionFunction {
		private readonly ExpressionFunctionCall functionCall;
		private readonly RankingArguments rankingArguments;

		[Rule("<ExpressionFunction> ::= <FunctionCall> OVER '(' <RankingArguments> ')'", ConstructorParameterMapping = new[] {0, 3})]
		public ExpressionRankingFunction(ExpressionFunctionCall functionCall, RankingArguments rankingArguments) {
			this.functionCall = functionCall;
			this.rankingArguments = rankingArguments;
		}
	}
}
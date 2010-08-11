using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionRankingFunction: ExpressionFunction {
		private readonly ExpressionFunctionCall functionCall;
		private readonly RankingArguments rankingArguments;

		[Rule("<ExpressionFunction> ::= <FunctionCall> OVER '(' <RankingArguments> ')'", ConstructorParameterMapping = new[] {0, 3})]
		public ExpressionRankingFunction(ExpressionFunctionCall functionCall, RankingArguments rankingArguments) {
			this.functionCall = functionCall;
			this.rankingArguments = rankingArguments;
		}

		public ExpressionFunctionCall FunctionCall {
			get {
				return functionCall;
			}
		}

		public RankingArguments RankingArguments {
			get {
				return rankingArguments;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(functionCall, WhitespacePadding.None);
			writer.Write(" OVER (");
			writer.WriteScript(rankingArguments, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}
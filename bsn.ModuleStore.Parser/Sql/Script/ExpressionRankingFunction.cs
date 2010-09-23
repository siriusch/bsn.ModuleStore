using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionRankingFunction: ExpressionFunction {
		private readonly FunctionCall functionCall;
		private readonly RankingArguments rankingArguments;

		[Rule("<Value> ::= <FunctionCall> ~OVER ~'(' <RankingArguments> ~')'")]
		public ExpressionRankingFunction(FunctionCall functionCall, RankingArguments rankingArguments) {
			this.functionCall = functionCall;
			this.rankingArguments = rankingArguments;
		}

		public FunctionCall FunctionCall {
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
			WriteCommentsTo(writer);
			writer.WriteScript(functionCall, WhitespacePadding.None);
			writer.Write(" OVER (");
			writer.WriteScript(rankingArguments, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class KeywordFunction: FunctionCall {
		private readonly ReservedKeyword keyword;

		[Rule("<FunctionCall> ::= CURRENT_TIMESTAMP")]
		[Rule("<FunctionCall> ::= CURRENT_USER")]
		[Rule("<FunctionCall> ::= SESSION_USER")]
		[Rule("<FunctionCall> ::= SYSTEM_USER")]
		[Rule("<FunctionCall> ::= USER")]
		public KeywordFunction(ReservedKeyword keyword) {
			Debug.Assert(keyword != null);
			this.keyword = keyword;
		}

		public ReservedKeyword Keyword {
			get {
				return keyword;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(keyword, WhitespacePadding.None);
		}
	}
}
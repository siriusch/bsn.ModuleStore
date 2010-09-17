using System.Collections.Generic;
using System.IO;
using System.Linq;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	internal class SqlSemanticProcessor: SemanticProcessor<SqlToken> {
		private readonly SqlTokenizer tokenizer;
		public SqlSemanticProcessor(TextReader reader, SemanticActions<SqlToken> actions): this(new SqlTokenizer(reader, actions), actions) {}

		public SqlSemanticProcessor(SqlTokenizer tokenizer, SemanticActions<SqlToken> actions): base(tokenizer, actions) {
			this.tokenizer = tokenizer;
		}

		protected override SqlToken CreateReduction(Rule rule, IList<SqlToken> children) {
			SqlToken result = base.CreateReduction(rule, children);
			CommentContainerToken commentToken = result as CommentContainerToken;
			if (commentToken != null) {
				foreach (IToken token in children) {
					if (token != null) {
						commentToken.AddComments(tokenizer.GetComments(token.Position.Index));
						break;
					}
				}
			}
			return result;
		}
	}
}
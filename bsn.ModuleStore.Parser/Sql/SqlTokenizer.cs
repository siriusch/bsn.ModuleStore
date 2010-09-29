using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql {
	internal class SqlTokenizer: Tokenizer<SqlToken> {
		private readonly SemanticActions<SqlToken> actions;
		private readonly List<string> pendingComments = new List<string>();
		private readonly List<KeyValuePair<int, string>> comments = new List<KeyValuePair<int, string>>();
		private bool repeatToken;
		private SqlToken lastToken;

		public SqlTokenizer(TextReader textReader, SemanticActions<SqlToken> actions): base(textReader, actions.Grammar) {
			this.actions = actions;
		}

		public override ParseMessage NextToken(out SqlToken token) {
			if (repeatToken) {
				repeatToken = false;
				Debug.Assert(lastToken != null);
				token = lastToken;
				lastToken = null;
				return ParseMessage.TokenRead;
			}
			ParseMessage parseMessage = base.NextToken(out token);
			lastToken = (parseMessage == ParseMessage.TokenRead) ? token : null;
			return parseMessage;
		}

		internal bool RepeatToken() {
			repeatToken = (lastToken != null);
			return repeatToken;
		}

		public IList<string> GetComments(int index) {
			int startIndex = comments.Count;
			while ((startIndex > 0) && (comments[startIndex-1].Key >= index)) {
				startIndex--;
			}
			int len = comments.Count-startIndex;
			if (len > 0) {
				List<string> result = new List<string>(len);
				for (int i = 0; i < len; i++) {
					result.Add(comments[startIndex+i].Value);
				}
				comments.RemoveRange(startIndex, len);
				return result;
			}
			return new string[0];
		}

		protected override SqlToken CreateToken(Symbol tokenSymbol, LineInfo tokenPosition, string text) {
			SemanticTerminalFactory<SqlToken> factory;
			if (!actions.TryGetTerminalFactory(tokenSymbol, out factory)) {
				throw new InvalidOperationException("No terminal factory for symbol "+tokenSymbol.Name);
			}
			SqlToken token = factory.CreateAndInitialize(tokenSymbol, tokenPosition, text);
			switch (tokenSymbol.Kind) {
			case SymbolKind.CommentLine:
			case SymbolKind.CommentStart:
				pendingComments.Add(text);
				break;
			case SymbolKind.Terminal:
				TransferPendingComments(tokenPosition.Index);
				break;
			case SymbolKind.End:
				TransferPendingComments(0);
				break;
			}
			return token;
		}

		private void TransferPendingComments(int index) {
			foreach (string comment in pendingComments) {
				comments.Add(new KeyValuePair<int, string>(index, comment));
			}
			pendingComments.Clear();
		}
	}
}
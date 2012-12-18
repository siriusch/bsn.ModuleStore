// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
		private readonly List<KeyValuePair<int, string>> comments = new List<KeyValuePair<int, string>>();
		private readonly List<string> pendingComments = new List<string>();
		private SqlToken lastToken;
		private bool repeatToken;

		public SqlTokenizer(TextReader textReader, SemanticActions<SqlToken> actions): base(textReader, actions.Grammar) {
			this.actions = actions;
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

		protected override SqlToken CreateToken(Symbol tokenSymbol, LineInfo tokenPosition, string text) {
			SemanticTerminalFactory<SqlToken> factory;
			if (!actions.TryGetTerminalFactory(tokenSymbol, out factory)) {
				throw new InvalidOperationException("No terminal factory for symbol "+tokenSymbol.Name);
			}
			SqlToken token = factory.CreateAndInitialize(tokenSymbol, tokenPosition, text);
			if (token is CommentToken) {
				pendingComments.Add(text);
			} else {
				switch (tokenSymbol.Kind) {
				case SymbolKind.Terminal:
					TransferPendingComments((int)tokenPosition.Index);
					break;
				case SymbolKind.End:
					TransferPendingComments(0);
					break;
				}
			}
			return token;
		}

		internal bool RepeatToken() {
			repeatToken = (lastToken != null);
			return repeatToken;
		}

		private void TransferPendingComments(int index) {
			foreach (string comment in pendingComments) {
				comments.Add(new KeyValuePair<int, string>(index, comment));
			}
			pendingComments.Clear();
		}
	}
}

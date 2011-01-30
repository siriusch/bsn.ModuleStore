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
//  
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
	internal class SqlSemanticProcessor: SemanticProcessor<SqlToken> {
		private readonly Symbol identifierSymbol;
		private readonly Symbol terminatorSymbol;
		private readonly SqlTokenizer tokenizer;

		public SqlSemanticProcessor(TextReader reader, SemanticActions<SqlToken> actions): this(new SqlTokenizer(reader, actions), actions) {}

		public SqlSemanticProcessor(SqlTokenizer tokenizer, SemanticActions<SqlToken> actions): base(tokenizer, actions) {
			terminatorSymbol = actions.Grammar.GetSymbolByName("';'");
			identifierSymbol = actions.Grammar.GetSymbolByName("Id");
			Debug.Assert(terminatorSymbol != null);
			this.tokenizer = tokenizer;
		}

		protected override SqlToken CreateReduction(Rule rule, IList<SqlToken> children) {
			SqlToken result = base.CreateReduction(rule, children);
			CommentContainerToken commentToken = result as CommentContainerToken;
			if (commentToken != null) {
				foreach (IToken token in children) {
					if ((token != null) && (token.Position.Line > 0)) {
						commentToken.AddComments(tokenizer.GetComments((int)token.Position.Index));
						break;
					}
				}
			}
			return result;
		}

		protected override bool RetrySyntaxError(ref SqlToken currentToken) {
			UnreservedKeyword unreservedKeyword = currentToken as UnreservedKeyword;
			if (unreservedKeyword != null) {
				currentToken = unreservedKeyword.AsIdentifier(identifierSymbol);
				return true;
			}
			if (((IToken)currentToken).Symbol != terminatorSymbol) {
				if (tokenizer.RepeatToken()) {
					InsignificantToken terminator = new InsignificantToken();
					terminator.InitializeInternal(terminatorSymbol, ((IToken)currentToken).Position);
					currentToken = terminator;
					return true;
				}
			}
			return false;
		}
	}
}

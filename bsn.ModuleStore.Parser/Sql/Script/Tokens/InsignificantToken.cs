using System;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("(EOF)")]
	[Terminal("(Error)")]
	[Terminal("(Whitespace)")]
	[Terminal("(Comment End)")]
	[Terminal("(Comment Line)")]
	[Terminal("(Comment Start)")]
	[Terminal("(")]
	[Terminal(")")]
	[Terminal(".")]
	[Terminal(":")]
	[Terminal(",")]
	[Terminal(";")]
	public class InsignificantToken: SqlToken {
		[Rule("<OptionalAs> ::= ~AS")]
		[Rule("<OptionalAs> ::=")]
		[Rule("<Terminator> ::= ~';'")]
		[Rule("<Terminator> ::= ~<Terminator> ~';'")]
		[Rule("<OptionalInto> ::=")]
		[Rule("<OptionalInto> ::= ~INTO")]
		[Rule("<OptionalFrom> ::=")]
		[Rule("<OptionalFrom> ::= ~FROM")]
		public InsignificantToken() {}

		internal void InitializeInternal(Symbol symbol, LineInfo lineInfo) {
			base.Initialize(symbol, lineInfo);
		}
	}
}
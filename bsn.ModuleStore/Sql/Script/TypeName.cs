using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TypeName: Name {
		[Rule("<TypeName> ::= Id")]
		public TypeName(Identifier identifier): base(identifier) {}

		[Rule("<TypeName> ::= Id '(' Id ')'")]
		[Rule("<TypeName> ::= Id '(' <IntegerLiteral> ')'")]
		public TypeName(Identifier identifier, InsignificantToken openBrace, SqlToken size, InsignificantToken closeBrace): this(identifier) {}

		[Rule("<TypeName> ::= Id '(' <IntegerLiteral> ',' <IntegerLiteral> ')'")]
		public TypeName(Identifier identifier, InsignificantToken openBrace, Literal precision, InsignificantToken comma, Literal scale, InsignificantToken closeBrace): this(identifier) {}
	}
}

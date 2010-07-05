using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TypeName: SqlName {
		[Rule("<TypeName> ::= Id")]
		public TypeName(SqlIdentifier identifier): base(identifier.Value) {}

		[Rule("<TypeName> ::= Id '(' Id ')'")]
		[Rule("<TypeName> ::= Id '(' <IntegerLiteral> ')'")]
		public TypeName(SqlIdentifier identifier, InsignificantToken openBrace, SqlToken size, InsignificantToken closeBrace): this(identifier) {}

		[Rule("<TypeName> ::= Id '(' <IntegerLiteral> ',' <IntegerLiteral> ')'")]
		public TypeName(SqlIdentifier identifier, InsignificantToken openBrace, Literal<int> precision, InsignificantToken comma, Literal<int> scale, InsignificantToken closeBrace): this(identifier) {}
	}
}
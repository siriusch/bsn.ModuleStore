using System;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class TypeName: SqlName {
		[Rule("<TypeName> ::= Id")]
		public TypeName(SqlIdentifier identifier): base(identifier.Value) {}

		[Rule("<TypeName> ::= Id '(' Id ')'")]
		[Rule("<TypeName> ::= Id '(' <IntegerLiteral> ')'")]
		public TypeName(SqlIdentifier identifier, InsignificantToken openBrace, SqlToken size, InsignificantToken closeBrace): this(identifier) {}

		[Rule("<TypeName> ::= Id '(' <IntegerLiteral> ',' <IntegerLiteral> ')'")]
		public TypeName(SqlIdentifier identifier, InsignificantToken openBrace, IntegerLiteral precision, InsignificantToken comma, IntegerLiteral scale, InsignificantToken closeBrace): this(identifier) {}
	}
}
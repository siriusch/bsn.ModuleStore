using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ColumnDefaultConstraint: ColumnConstraint {
		[Rule("<ColumnConstraint> ::= DEFAULT '(' <FunctionCall> ')'", ConstructorParameterMapping = new[] {2})]
		[Rule("<ColumnConstraint> ::= DEFAULT <NumberLiteral>", ConstructorParameterMapping = new[] {1})]
		[Rule("<ColumnConstraint> ::= DEFAULT <StringLiteral>", ConstructorParameterMapping = new[] {1})]
		[Rule("<ColumnConstraint> ::= DEFAULT <NullLiteral>", ConstructorParameterMapping = new[] {1})]
		public ColumnDefaultConstraint(Expression defaultValue) {}
	}
}
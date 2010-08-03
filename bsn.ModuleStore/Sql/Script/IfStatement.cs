using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class IfStatement: SqlStatement {
		private readonly Predicate condition;
		private readonly SqlStatement elseStatement;
		private readonly SqlStatement thenStatement;

		[Rule("<IfStatement> ::= IF <Predicate> THEN <StatementGroup>", ConstructorParameterMapping = new[] {1, 3})]
		[Rule("<IfStatement> ::= IF <Predicate> <Statement>", ConstructorParameterMapping = new[] {1, 2})]
		public IfStatement(Predicate condition, SqlStatement thenStatement): this(condition, thenStatement, null) {}

		[Rule("<IfStatement> ::= IF <Predicate> THEN <StatementBlock> ELSE <StatementGroup>", ConstructorParameterMapping = new[] {1, 3, 5})]
		public IfStatement(Predicate condition, SqlStatement thenStatement, SqlStatement elseStatement) {
			if (condition == null) {
				throw new ArgumentNullException("condition");
			}
			if (thenStatement == null) {
				throw new ArgumentNullException("thenStatement");
			}
			this.condition = condition;
			this.thenStatement = thenStatement;
			this.elseStatement = elseStatement;
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("IF ");
			condition.WriteTo(writer);
			writer.Write(" THEN ");
			thenStatement.WriteTo(writer);
			if (elseStatement != null) {
				writer.Write(" ELSE ");
				elseStatement.WriteTo(writer);
			}
		}
	}
}
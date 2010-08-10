using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class IfStatement: Statement {
		private readonly Predicate condition;
		private readonly Statement elseStatement;
		private readonly Statement thenStatement;

		[Rule("<IfStatement> ::= IF <Predicate> THEN <StatementGroup>", ConstructorParameterMapping = new[] {1, 3})]
		[Rule("<IfStatement> ::= IF <Predicate> <Statement>", ConstructorParameterMapping = new[] {1, 2})]
		public IfStatement(Predicate condition, Statement thenStatement): this(condition, thenStatement, null) {}

		[Rule("<IfStatement> ::= IF <Predicate> THEN <StatementBlock> ELSE <StatementGroup>", ConstructorParameterMapping = new[] {1, 3, 5})]
		public IfStatement(Predicate condition, Statement thenStatement, Statement elseStatement) {
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

		public Predicate Condition {
			get {
				return condition;
			}
		}

		public Statement ElseStatement {
			get {
				return elseStatement;
			}
		}

		public Statement ThenStatement {
			get {
				return thenStatement;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("IF ");
			writer.WriteScript(condition);
			writer.Write(" THEN ");
			writer.WriteScript(thenStatement);
			writer.WriteScript(elseStatement, " ELSE ", null);
		}
	}
}
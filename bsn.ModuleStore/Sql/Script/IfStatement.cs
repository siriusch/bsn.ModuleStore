using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class IfStatement: Statement {
		private readonly Predicate condition;
		private readonly Statement elseStatement;
		private readonly Statement thenStatement;

		[Rule("<IfStatement> ::= IF <Predicate> <StatementGroup>", ConstructorParameterMapping = new[] {1, 2})]
		public IfStatement(Predicate condition, Statement thenStatement): this(condition, thenStatement, null) {}

		[Rule("<IfStatement> ::= IF <Predicate> <StatementBlock> ELSE <StatementGroup>", ConstructorParameterMapping = new[] {1, 2, 4})]
		public IfStatement(Predicate condition, Statement thenStatement, Statement elseStatement) {
			Debug.Assert(condition != null);
			Debug.Assert(thenStatement != null);
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

		public override void WriteTo(SqlWriter writer) {
			writer.Write("IF ");
			writer.WriteScript(condition, WhitespacePadding.SpaceAfter);
			writer.WriteScript(thenStatement, WhitespacePadding.None);
			writer.WriteScript(elseStatement, WhitespacePadding.None, " ELSE ", null);
		}
	}
}
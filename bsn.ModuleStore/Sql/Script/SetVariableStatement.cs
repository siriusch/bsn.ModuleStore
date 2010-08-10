using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SetVariableStatement: Statement {
		private readonly Expression expression;
		private readonly VariableName variable;

		[Rule("<SetVariableStatement> ::= SET <VariableName> '=' <Expression>", ConstructorParameterMapping = new[] {1, 3})]
		public SetVariableStatement(VariableName variable, Expression expression) {
			if (variable == null) {
				throw new ArgumentNullException("variable");
			}
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
			this.variable = variable;
			this.expression = expression;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public VariableName Variable {
			get {
				return variable;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("SET ");
			writer.WriteScript(variable);
			writer.Write("=");
			writer.WriteScript(expression);
		}
	}
}
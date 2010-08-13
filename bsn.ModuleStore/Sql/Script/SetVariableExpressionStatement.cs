using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SetVariableExpressionStatement: SetVariableStatement {
		private readonly Expression expression;

		[Rule("<SetVariableStatement> ::= SET <VariableName> '=' <Expression>", ConstructorParameterMapping = new[] {1, 3})]
		public SetVariableExpressionStatement(VariableName variableName, Expression expression): base(variableName) {
			this.expression = expression;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteScript(expression, WhitespacePadding.None);
		}
	}
}
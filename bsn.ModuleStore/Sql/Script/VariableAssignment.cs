using System;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class VariableAssignment: SqlScriptableToken {
		private readonly Expression expression;
		private readonly VariableName variableName;

		[Rule("<VariableAssignment> ::= <VariableName> '=' <Expression>", ConstructorParameterMapping = new[] {0, 2})]
		public VariableAssignment(VariableName variableName, Expression expression) {
			Debug.Assert(variableName != null);
			Debug.Assert(expression != null);
			this.variableName = variableName;
			this.expression = expression;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public VariableName VariableName {
			get {
				return variableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(variableName, WhitespacePadding.None);
			writer.Write('=');
			writer.WriteScript(expression, WhitespacePadding.None);
		}
	}
}
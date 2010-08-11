using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class VariableAssignment: SqlToken, IScriptable {
		private readonly VariableName variableName;
		private readonly Expression expression;

		[Rule("<VariableAssignment> ::= <VariableName> '=' <Expression>", ConstructorParameterMapping = new[] {0, 2})]
		public VariableAssignment(VariableName variableName, Expression expression) {
			if (variableName == null) {
				throw new ArgumentNullException("variableName");
			}
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
			this.variableName = variableName;
			this.expression = expression;
		}

		public VariableName VariableName {
			get {
				return variableName;
			}
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public void WriteTo(TextWriter writer) {
			writer.WriteScript(variableName);
			writer.Write('=');
			writer.WriteScript(expression);
		}
	}
}

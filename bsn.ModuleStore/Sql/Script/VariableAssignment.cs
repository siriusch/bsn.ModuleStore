using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			Debug.Assert(variableName != null);
			Debug.Assert(expression != null);
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

		public void WriteTo(SqlWriter writer) {
			writer.WriteScript(variableName, WhitespacePadding.None);
			writer.Write('=');
			writer.WriteScript(expression, WhitespacePadding.None);
		}
	}
}

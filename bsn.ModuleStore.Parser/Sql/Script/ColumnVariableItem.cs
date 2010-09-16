using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnVariableItem: ColumnItem {
		private readonly Expression expression;
		private readonly VariableName variableName;

		[Rule("<ColumnItem> ::= <VariableName> ~'=' <Expression>")]
		public ColumnVariableItem(VariableName variableName, Expression expression) {
			Debug.Assert(expression != null);
			Debug.Assert(variableName != null);
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
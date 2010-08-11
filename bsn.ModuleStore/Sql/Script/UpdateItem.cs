using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class UpdateItem: SqlToken, IScriptable {
		private readonly Qualified<ColumnName> columnName;
		private readonly Expression expression;
		private readonly VariableName variableName;

		[Rule("<UpdateItem> ::= <ColumnNameQualified> '=' DEFAULT", ConstructorParameterMapping = new[] {0})]
		public UpdateItem(Qualified<ColumnName> columnName): this(null, columnName, null) {}

		[Rule("<UpdateItem> ::= <VariableName> '=' <Expression>", ConstructorParameterMapping = new[] {0, 2})]
		public UpdateItem(VariableName variableName, Expression expression): this(variableName, null, expression) {}

		[Rule("<UpdateItem> ::= <ColumnNameQualified> '=' <Expression>", ConstructorParameterMapping = new[] {0, 2})]
		public UpdateItem(Qualified<ColumnName> columnName, Expression expression): this(null, columnName, expression) {}

		[Rule("<UpdateItem> ::= <VariableName> '=' <ColumnNameQualified> '=' <Expression>", ConstructorParameterMapping = new[] {0, 2, 4})]
		public UpdateItem(VariableName variableName, Qualified<ColumnName> columnName, Expression expression) {
			this.columnName = columnName;
			this.variableName = variableName;
			this.expression = expression;
		}

		public Qualified<ColumnName> ColumnName {
			get {
				return columnName;
			}
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

		public void WriteTo(SqlWriter writer) {
			writer.WriteScript(variableName, null, "=");
			writer.WriteScript(columnName, null, "=");
			if (expression == null) {
				writer.Write("DEFAULT");
			} else {
				writer.WriteScript(expression);
			}
		}
	}
}
using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class UpdateItem: SqlToken {
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
	}
}
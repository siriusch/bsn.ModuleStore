using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateViewStatement: SqlCreateStatement {
		private readonly Optional<Sequence<ColumnName>> columnNames;
		private readonly SelectStatement selectStatement;
		private readonly ViewName viewName;
		private readonly Optional<WithCheckOption> withCheckOption;
		private readonly Optional<WithViewMetadata> withViewMetadata;

		[Rule("<CreateViewStatement> ::= CREATE VIEW <ViewName> <ColumnNameGroup> <ViewOptionalAttribute> AS <SelectStatement> <ViewOptionalCheckOption>", ConstructorParameterMapping = new[] {2, 3, 4, 6, 7})]
		public CreateViewStatement(ViewName viewName, Optional<Sequence<ColumnName>> columnNames, Optional<WithViewMetadata> withViewMetadata, SelectStatement selectStatement, Optional<WithCheckOption> withCheckOption) {
			this.viewName = viewName;
			this.columnNames = columnNames;
			this.withViewMetadata = withViewMetadata;
			this.selectStatement = selectStatement;
			this.withCheckOption = withCheckOption;
		}
	}
}
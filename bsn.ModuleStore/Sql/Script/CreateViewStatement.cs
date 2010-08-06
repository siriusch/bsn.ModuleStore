using System;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateViewStatement: SqlCreateStatement {
		private readonly Optional<Sequence<ColumnName>> columnNames;
		private readonly SelectStatement selectStatement;
		private readonly ViewName viewName;
		private readonly Optional<WithCheckOptionToken> withCheckOption;
		private readonly Optional<WithViewMetadataToken> withViewMetadata;

		[Rule("<CreateViewStatement> ::= CREATE VIEW <ViewName> <ColumnNameGroup> <ViewOptionalAttribute> AS <SelectStatement> <ViewOptionalCheckOption>", ConstructorParameterMapping = new[] {2, 3, 4, 6, 7})]
		public CreateViewStatement(ViewName viewName, Optional<Sequence<ColumnName>> columnNames, Optional<WithViewMetadataToken> withViewMetadata, SelectStatement selectStatement, Optional<WithCheckOptionToken> withCheckOption) {
			this.viewName = viewName;
			this.columnNames = columnNames;
			this.withViewMetadata = withViewMetadata;
			this.selectStatement = selectStatement;
			this.withCheckOption = withCheckOption;
		}
	}
}
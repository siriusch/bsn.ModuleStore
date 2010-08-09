using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateViewStatement: SqlCreateStatement {
		private readonly List<ColumnName> columnNames;
		private readonly SelectStatement selectStatement;
		private readonly ViewName viewName;
		private readonly bool withCheckOption;
		private readonly bool withViewMetadata;

		[Rule("<CreateViewStatement> ::= CREATE VIEW <ViewName> <ColumnNameGroup> <ViewOptionalAttribute> AS <SelectStatement> <ViewOptionalCheckOption>", ConstructorParameterMapping = new[] {2, 3, 4, 6, 7})]
		public CreateViewStatement(ViewName viewName, Optional<Sequence<ColumnName>> columnNames, Optional<WithViewMetadataToken> withViewMetadata, SelectStatement selectStatement, Optional<WithCheckOptionToken> withCheckOption) {
			if (viewName == null) {
				throw new ArgumentNullException("viewName");
			}
			if (selectStatement == null) {
				throw new ArgumentNullException("selectStatement");
			}
			this.viewName = viewName;
			this.columnNames = columnNames.ToList();
			this.withViewMetadata = withViewMetadata.HasValue();
			this.selectStatement = selectStatement;
			this.withCheckOption = withCheckOption.HasValue();
		}

		public List<ColumnName> ColumnNames {
			get {
				return columnNames;
			}
		}

		public SelectStatement SelectStatement {
			get {
				return selectStatement;
			}
		}

		public ViewName ViewName {
			get {
				return viewName;
			}
		}

		public bool WithCheckOption {
			get {
				return withCheckOption;
			}
		}

		public bool WithViewMetadata {
			get {
				return withViewMetadata;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("CREATE VIEW ");
			writer.WriteScript(viewName);
			if (columnNames.Count > 0) {
				writer.Write(" (");
				writer.WriteSequence(columnNames, null, ", ", null);
				writer.Write(')');
			}
			writer.WriteWithViewMetadata(withViewMetadata, " ", null);
			writer.WriteLine(" AS");
			writer.WriteScript(selectStatement);
			writer.WriteWithCheckOption(withCheckOption, " ", null);
		}
	}
}
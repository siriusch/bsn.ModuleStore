using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateViewStatement: CreateStatement {
		private readonly List<ColumnName> columnNames;
		private readonly SelectStatement selectStatement;
		private readonly Qualified<SchemaName, ViewName> viewName;
		private readonly bool withCheckOption;
		private readonly bool withViewMetadata;

		[Rule("<CreateViewStatement> ::= CREATE VIEW <ViewNameQualified> <ColumnNameGroup> <ViewOptionalAttribute> AS <SelectStatement> <ViewOptionalCheckOption>", ConstructorParameterMapping = new[] {2, 3, 4, 6, 7})]
		public CreateViewStatement(Qualified<SchemaName, ViewName> viewName, Optional<Sequence<ColumnName>> columnNames, Optional<WithViewMetadataToken> withViewMetadata, SelectStatement selectStatement, Optional<WithCheckOptionToken> withCheckOption) {
			Debug.Assert(viewName != null);
			Debug.Assert(selectStatement != null);
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

		public override sealed ObjectCategory ObjectCategory {
			get {
				return ObjectCategory.View;
			}
		}

		public override string ObjectName {
			get {
				return viewName.Name.Value;
			}
		}

		public SelectStatement SelectStatement {
			get {
				return selectStatement;
			}
		}

		public Qualified<SchemaName, ViewName> ViewName {
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

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CREATE VIEW ");
			writer.WriteScript(viewName, WhitespacePadding.None);
			if (columnNames.Count > 0) {
				writer.Write(" (");
				writer.WriteScriptSequence(columnNames, WhitespacePadding.None, ", ");
				writer.Write(')');
			}
			if (withViewMetadata) {
				writer.Write(" WITH VIEW_METADATA");
			}
			writer.WriteLine(" AS");
			writer.WriteScript(selectStatement, WhitespacePadding.None);
			if (withCheckOption) {
				writer.Write(" WITH CHECK OPTION");
			}
		}
	}
}
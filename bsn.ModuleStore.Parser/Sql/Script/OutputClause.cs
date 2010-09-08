using System;
using System.Collections.Generic;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class OutputClause: SqlScriptableToken, IOptional {
		private readonly List<ColumnItem> columnItems;
		private readonly List<ColumnName> destinationColumnNames;
		private readonly DestinationRowset destinationName;

		[Rule("<OutputClause> ::=")]
		public OutputClause(): this(null, null, null) {}

		[Rule("<OutputClause> ::= OUTPUT <ColumnItemList>", ConstructorParameterMapping = new[] {1})]
		public OutputClause(Sequence<ColumnItem> columnItems): this(columnItems, null, null) {}

		[Rule("<OutputClause> ::= OUTPUT <ColumnItemList> INTO <DestinationRowset> <ColumnNameGroup>", ConstructorParameterMapping = new[] {1, 3, 4})]
		public OutputClause(Sequence<ColumnItem> columnItems, DestinationRowset destinationName, Optional<Sequence<ColumnName>> destinationColumnNames) {
			this.destinationName = destinationName;
			this.destinationColumnNames = destinationColumnNames.ToList();
			this.columnItems = columnItems.ToList();
		}

		public IEnumerable<ColumnItem> ColumnItems {
			get {
				return columnItems;
			}
		}

		public IEnumerable<ColumnName> DestinationColumnNames {
			get {
				return destinationColumnNames;
			}
		}

		public DestinationRowset DestinationName {
			get {
				return destinationName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			if (HasValue) {
				writer.Write("OUTPUT ");
				writer.WriteScriptSequence(columnItems, WhitespacePadding.None, ", ");
				if (destinationName != null) {
					writer.Write(" INTO ");
					writer.WriteScript(destinationName, WhitespacePadding.None);
					if (destinationColumnNames.Count > 0) {
						writer.Write(" (");
						writer.WriteScriptSequence(destinationColumnNames, WhitespacePadding.None, ", ");
						writer.Write(")");
					}
				}
			}
		}

		public bool HasValue {
			get {
				return columnItems.Count > 0;
			}
		}
	}
}
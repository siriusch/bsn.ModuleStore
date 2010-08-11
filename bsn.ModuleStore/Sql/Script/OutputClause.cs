using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class OutputClause: SqlToken, IScriptable, IOptional {
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

		public List<ColumnItem> ColumnItems {
			get {
				return columnItems;
			}
		}

		public List<ColumnName> DestinationColumnNames {
			get {
				return destinationColumnNames;
			}
		}

		public DestinationRowset DestinationName {
			get {
				return destinationName;
			}
		}

		public bool HasValue {
			get {
				return columnItems.Count > 0;
			}
		}

		public void WriteTo(SqlWriter writer) {
			if (HasValue) {
				writer.Write("OUTPUT ");
				writer.WriteSequence(columnItems, null, ", ", null);
				if (destinationName != null) {
					writer.Write(" INTO ");
					writer.WriteScript(destinationName);
					if (destinationColumnNames.Count > 0) {
						writer.Write(" (");
						writer.WriteSequence(destinationColumnNames, null, ", ", null);
						writer.Write(")");
					}
				}
			}
		}
	}
}
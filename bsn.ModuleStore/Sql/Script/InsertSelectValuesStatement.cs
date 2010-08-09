using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class InsertSelectValuesStatement: InsertValuesStatement {
		private readonly SelectQuery selectQuery;

		[Rule("<InsertStatement> ::= <CTEGroup> INSERT <Top> <OptionalInto> <DestinationRowset> <ColumnNameGroup> <OutputClause> <SelectQuery>", ConstructorParameterMapping = new[] {0, 2, 4, 5, 6, 7})]
		public InsertSelectValuesStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output, SelectQuery selectQuery)
				: base(ctes, topExpression, destinationRowset, columnNames, output) {
			if (selectQuery == null) {
				throw new ArgumentNullException("selectQuery");
			}
			this.selectQuery = selectQuery;
		}

		public SelectQuery SelectQuery {
			get {
				return selectQuery;
			}
		}

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			writer.WriteScript(selectQuery);
		}
	}
}
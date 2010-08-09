using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class InsertExecuteValuesStatement: InsertValuesStatement {
		private readonly ExecuteStatement executeStatement;

		[Rule("<InsertStatement> ::= <CTEGroup> INSERT <Top> <OptionalInto> <DestinationRowset> <ColumnNameGroup> <OutputClause> <ExecuteStatement>", ConstructorParameterMapping = new[] {0, 2, 4, 5, 6, 7})]
		public InsertExecuteValuesStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output, ExecuteStatement executeStatement)
				: base(ctes, topExpression, destinationRowset, columnNames, output) {
			if (executeStatement == null) {
				throw new ArgumentNullException("executeStatement");
			}
			this.executeStatement = executeStatement;
		}

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			writer.WriteScript(executeStatement, null, null);
		}
	}
}
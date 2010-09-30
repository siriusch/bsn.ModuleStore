using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	internal class FulltextMultipleTableFunction: FulltextTableFunction {
		private readonly List<ColumnName> columns;

		[Rule("<TextTableFunction> ::= CONTAINSTABLE ~'(' <TableNameQualified> ~',' ~'(' <ColumnNameList> ~')' ~',' <Expression> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= FREETEXTTABLE ~'(' <TableNameQualified> ~',' ~'(' <ColumnNameList> ~')' ~',' <Expression> <OptionalContainsTop> ~')'")]
		public FulltextMultipleTableFunction(ReservedKeyword keyword, Qualified<SchemaName, TableName> tableName, Sequence<ColumnName> columns, Expression query, Optional<IntegerLiteral> top): this(keyword, tableName, columns, query, null, top) {}

		[Rule("<TextTableFunction> ::= CONTAINSTABLE ~'(' <TableNameQualified> ~',' ~'(' <ColumnNameList> ~')' ~',' <Expression> ~',' ~LANGUAGE <Literal> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= FREETEXTTABLE ~'(' <TableNameQualified> ~',' ~'(' <ColumnNameList> ~')' ~',' <Expression> ~',' ~LANGUAGE <Literal> <OptionalContainsTop> ~')'")]
		public FulltextMultipleTableFunction(ReservedKeyword keyword, Qualified<SchemaName, TableName> tableName, Sequence<ColumnName> columns, Expression query, Literal language, Optional<IntegerLiteral> top): base(keyword, tableName, query, language, top) {
			Debug.Assert(columns != null);
			this.columns = columns.ToList();
		}

		public IEnumerable<ColumnName> Columns {
			get {
				return columns;
			}
		}

		protected override void WriteColumnInternal(SqlWriter writer) {
			writer.WriteScriptSequence(columns, WhitespacePadding.None, ", ");
		}
	}
}
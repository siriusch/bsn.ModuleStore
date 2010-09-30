using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	internal class FulltextSingleTableFunction: FulltextTableFunction {
		private readonly ColumnName column;

		[Rule("<TextTableFunction> ::= CONTAINSTABLE ~'(' <TableNameQualified> ~',' <ColumnName> ~',' <Expression> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= CONTAINSTABLE ~'(' <TableNameQualified> ~',' <ColumnWild> ~',' <Expression> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= FREETEXTTABLE ~'(' <TableNameQualified> ~',' <ColumnName> ~',' <Expression> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= FREETEXTTABLE ~'(' <TableNameQualified> ~',' <ColumnWild> ~',' <Expression> <OptionalContainsTop> ~')'")]
		public FulltextSingleTableFunction(ReservedKeyword keyword, Qualified<SchemaName, TableName> tableName, ColumnName column, Expression query, Optional<IntegerLiteral> top): this(keyword, tableName, column, query, null, top) {}

		[Rule("<TextTableFunction> ::= CONTAINSTABLE ~'(' <TableNameQualified> ~',' <ColumnName> ~',' <Expression> ~',' ~LANGUAGE <Literal> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= CONTAINSTABLE ~'(' <TableNameQualified> ~',' <ColumnWild> ~',' <Expression> ~',' ~LANGUAGE <Literal> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= FREETEXTTABLE ~'(' <TableNameQualified> ~',' <ColumnName> ~',' <Expression> ~',' ~LANGUAGE <Literal> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= FREETEXTTABLE ~'(' <TableNameQualified> ~',' <ColumnWild> ~',' <Expression> ~',' ~LANGUAGE <Literal> <OptionalContainsTop> ~')'")]
		public FulltextSingleTableFunction(ReservedKeyword keyword, Qualified<SchemaName, TableName> tableName, ColumnName column, Expression query, Literal language, Optional<IntegerLiteral> top): base(keyword, tableName, query, language, top) {
			Debug.Assert(column != null);
			this.column = column;
		}

		public ColumnName Column {
			get {
				return column;
			}
		}

		protected override void WriteColumnInternal(SqlWriter writer) {
			writer.WriteScript(column, WhitespacePadding.None);
		}
	}
}
using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	internal class FulltextSingleFunction: FulltextFunction {
		private readonly Qualified<SqlName, ColumnName> column;

		[Rule("<PredicateFunction> ::= CONTAINS ~'(' <ColumnWildNameQualified> ~',' <Expression> ~')'")]
		[Rule("<PredicateFunction> ::= FREETEXT ~'(' <ColumnWildNameQualified> ~',' <Expression> ~')'")]
		public FulltextSingleFunction(ReservedKeyword keyword, Qualified<SqlName, ColumnName> column, Expression query): this(keyword, column, query, null) {}

		[Rule("<PredicateFunction> ::= CONTAINS ~'(' <ColumnWildNameQualified> ~',' <Expression> ~',' ~LANGUAGE <Literal> ~')'")]
		[Rule("<PredicateFunction> ::= FREETEXT ~'(' <ColumnWildNameQualified> ~',' <Expression> ~',' ~LANGUAGE <Literal> ~')'")]
		public FulltextSingleFunction(ReservedKeyword keyword, Qualified<SqlName, ColumnName> column, Expression query, Literal language): base(keyword, query, language) {
			Debug.Assert(column != null);
			this.column = column;
		}

		public Qualified<SqlName, ColumnName> Column {
			get {
				return column;
			}
		}

		protected override void WriteColumnInternal(SqlWriter writer) {
			writer.WriteScript(column, WhitespacePadding.None);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	internal class FulltextMultipleFunction: FulltextFunction {
		private readonly List<Qualified<SqlName, ColumnName>> columns;

		[Rule("<PredicateFunction> ::= CONTAINS ~'(' ~'(' <ColumnNameQualifiedList> ~')' ~',' <Expression> ~')'")]
		[Rule("<PredicateFunction> ::= FREETEXT ~'(' ~'(' <ColumnNameQualifiedList> ~')' ~',' <Expression> ~')'")]
		public FulltextMultipleFunction(ReservedKeyword keyword, Sequence<Qualified<SqlName, ColumnName>> columns, Expression query): this(keyword, columns, query, null) {}

		[Rule("<PredicateFunction> ::= CONTAINS ~'(' ~'(' <ColumnNameQualifiedList> ~')' ~',' <Expression> ~',' ~LANGUAGE <Literal> ~')'")]
		[Rule("<PredicateFunction> ::= FREETEXT ~'(' ~'(' <ColumnNameQualifiedList> ~')' ~',' <Expression> ~',' ~LANGUAGE <Literal> ~')'")]
		public FulltextMultipleFunction(ReservedKeyword keyword, Sequence<Qualified<SqlName, ColumnName>> columns, Expression query, Literal language): base(keyword, query, language) {
			Debug.Assert(columns != null);
			this.columns = columns.ToList();
		}

		public IEnumerable<Qualified<SqlName, ColumnName>> Columns {
			get {
				return columns;
			}
		}

		protected override void WriteColumnInternal(SqlWriter writer) {
			writer.WriteScriptSequence(columns, WhitespacePadding.None, ", ");
		}
	}
}
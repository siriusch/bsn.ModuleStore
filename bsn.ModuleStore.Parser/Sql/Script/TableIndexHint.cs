using System;
using System.Collections.Generic;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TableIndexHint: TableHint {
		private readonly List<IntegerLiteral> indexValues;

		[Rule("<TableHint> ::= ~INDEX ~'(' <IndexValueList> ~')'")]
		public TableIndexHint(Sequence<IntegerLiteral> indexValues) {
			this.indexValues = indexValues.ToList();
		}

		public IEnumerable<IntegerLiteral> IndexValues {
			get {
				return indexValues;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("INDEX(");
			writer.WriteScriptSequence(indexValues, WhitespacePadding.None, ", ");
			writer.Write(')');
		}
	}
}
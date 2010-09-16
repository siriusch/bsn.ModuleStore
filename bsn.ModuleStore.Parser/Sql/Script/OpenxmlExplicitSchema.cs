using System;
using System.Collections.Generic;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class OpenxmlExplicitSchema: OpenxmlSchema {
		private readonly List<OpenxmlColumn> columns;

		[Rule("<OpenxmlExplicitSchema> ::= ~WITH ~'(' <OpenxmlColumnList> ~')'")]
		public OpenxmlExplicitSchema(Sequence<OpenxmlColumn> columns) {
			this.columns = columns.ToList();
		}

		public IEnumerable<OpenxmlColumn> Columns {
			get {
				return columns;
			}
		}

		protected override void WriteToInternal(SqlWriter writer) {
			writer.WriteScriptSequence(columns, WhitespacePadding.None, ", ");
		}
	}
}
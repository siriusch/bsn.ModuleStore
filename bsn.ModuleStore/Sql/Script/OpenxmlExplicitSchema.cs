using System;
using System.Collections.Generic;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class OpenxmlExplicitSchema: OpenxmlSchema {
		private readonly List<OpenxmlColumn> columns;

		[Rule("<OpenxmlExplicitSchema> ::= WITH '(' <OpenxmlColumnList> ')'", ConstructorParameterMapping = new[] {2})]
		public OpenxmlExplicitSchema(Sequence<OpenxmlColumn> columns) {
			this.columns = columns.ToList();
		}

		public List<OpenxmlColumn> Columns {
			get {
				return columns;
			}
		}

		protected override void WriteToInternal(SqlWriter writer) {
			writer.WriteSequence(columns, WhitespacePadding.None, ", ");
		}
	}
}
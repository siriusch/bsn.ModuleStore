using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("HOLDLOCK")]
	public sealed class TableSimpleHint: TableHint {
		private readonly string id;

		public TableSimpleHint(string id) {
			Debug.Assert(!string.IsNullOrEmpty(id));
			this.id = id;
		}

		[Rule("<TableHint> ::= Id")]
		public TableSimpleHint(SqlIdentifier id): this(id.Value) {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write(id);
		}
	}
}
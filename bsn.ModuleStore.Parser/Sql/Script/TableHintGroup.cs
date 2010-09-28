using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TableHintGroup: SqlScriptableToken, IOptional {
		private readonly List<TableHint> hints;

		[Rule("<TableHintGroup> ::=")]
		public TableHintGroup(): this(null) {}

		[Rule("<TableHintGroup> ::= ~WITH ~'(' <TableHintList> ~')'")]
		public TableHintGroup(Sequence<TableHint> hints) {
			this.hints = hints.ToList();
		}

		public IEnumerable<TableHint> Hints {
			get {
				return hints;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			Debug.Assert(hints.Count > 0);
			writer.Write("WITH (");
			writer.WriteScriptSequence(hints, WhitespacePadding.None, ", ");
			writer.Write(')');
		}

		public bool HasValue {
			get {
				return hints.Count > 0;
			}
		}
	}
}
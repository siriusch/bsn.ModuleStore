using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class UpdateModeForUpdate: UpdateMode {
		private readonly List<ColumnName> columns = new List<ColumnName>();

		[Rule("<CursorUpdate> ::= ~FOR ~UPDATE")]
		public UpdateModeForUpdate() {}

		[Rule("<CursorUpdate> ::= ~FOR ~UPDATE ~OF <ColumnNameList>")]
		public UpdateModeForUpdate(Sequence<ColumnName> columns): base() {
			Debug.Assert(columns != null);
			this.columns.AddRange(columns);
		}

		public IEnumerable<ColumnName> Columns {
			get {
				return columns;
			}
		}

		public override UpdateModeKind Kind {
			get {
				return UpdateModeKind.ForUpdate;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("FOR UPDATE");
			if (columns.Count > 0) {
				writer.Write(" OF ");
				writer.WriteScriptSequence(columns, WhitespacePadding.None, ", ");
			}
		}
	}
}
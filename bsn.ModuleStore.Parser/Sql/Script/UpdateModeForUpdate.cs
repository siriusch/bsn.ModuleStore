using System;
using System.Collections.Generic;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class UpdateModeForUpdate: UpdateMode {
		private readonly List<ColumnName> columns;

		[Rule("<CursorUpdate> ::= ~FOR_UPDATE")]
		public UpdateModeForUpdate(): this(null) {}

		[Rule("<CursorUpdate> ::= ~FOR_UPDATE ~OF <ColumnNameList>")]
		public UpdateModeForUpdate(Sequence<ColumnName> columns): base() {
			this.columns = columns.ToList();
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
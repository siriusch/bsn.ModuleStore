using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SourceRowset: Source {
		private readonly RowsetAlias rowsetAlias;

		protected SourceRowset(RowsetAlias rowsetAlias) {
			this.rowsetAlias = rowsetAlias;
		}

		public RowsetAlias RowsetAlias {
			get {
				return rowsetAlias;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(rowsetAlias, WhitespacePadding.SpaceBefore);
		}
	}
}
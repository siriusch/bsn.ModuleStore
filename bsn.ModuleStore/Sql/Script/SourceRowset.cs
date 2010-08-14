using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SourceRowset: Source {
		private readonly AliasName aliasName;

		protected SourceRowset(AliasName aliasName) {
			this.aliasName = aliasName;
		}

		public AliasName AliasName {
			get {
				return aliasName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(aliasName, WhitespacePadding.SpaceBefore, "AS ", null);
		}
	}
}
using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SourceRowset: SqlToken, IScriptable {
		private readonly AliasName aliasName;

		protected SourceRowset(AliasName aliasName) {
			this.aliasName = aliasName;
		}

		public AliasName AliasName {
			get {
				return aliasName;
			}
		}

		public virtual void WriteTo(TextWriter writer) {
			writer.WriteScript(aliasName, " AS ", null);
		}
	}
}
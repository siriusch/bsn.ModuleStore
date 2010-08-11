using System;
using System.Diagnostics;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class IndexOption: SqlToken, IScriptable {
		private readonly Identifier key;

		protected IndexOption(Identifier key) {
			Debug.Assert(key != null);
			this.key = key;
		}

		public Identifier Key {
			get {
				return key;
			}
		}

		public virtual void WriteTo(SqlWriter writer) {
			writer.WriteScript(key);
			writer.Write("=");
		}
	}
}
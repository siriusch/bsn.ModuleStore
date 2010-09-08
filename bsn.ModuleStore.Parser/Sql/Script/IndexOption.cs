using System;
using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class IndexOption: SqlScriptableToken {
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

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(key, WhitespacePadding.None);
			writer.Write("=");
		}
	}
}
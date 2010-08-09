using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class IndexOption: SqlToken, IScriptable {
		private readonly Identifier key;

		protected IndexOption(Identifier key) {
			if (key == null) {
				throw new ArgumentNullException("key");
			}
			this.key = key;
		}

		public Identifier Key {
			get {
				return key;
			}
		}

		public virtual void WriteTo(TextWriter writer) {
			writer.WriteScript(key);
			writer.Write("=");
		}
	}
}
using System;
using System.Diagnostics;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlIdentifier: SqlScriptableToken {
		private readonly string original;

		protected SqlIdentifier(string id) {
			Debug.Assert(id != null);
			original = id;
		}

		public virtual string Value {
			get {
				return original;
			}
		}

		protected string Original {
			get {
				return original;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write(original);
		}
	}
}
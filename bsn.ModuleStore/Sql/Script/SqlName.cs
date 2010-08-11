using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlName: SqlToken, IScriptable {
		private readonly string value;

		protected SqlName(string name) {
			Debug.Assert(!string.IsNullOrEmpty(name));
			value = name;
		}

		public string Value {
			get {
				return value;
			}
		}

		public virtual void WriteTo(SqlWriter writer) {
			writer.Write(value);
		}
	}
}
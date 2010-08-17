using System;
using System.Diagnostics;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlName: SqlScriptableToken {
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

		protected internal virtual void WriteToInternal(SqlWriter writer, bool isPartOfQualifiedName) {
			writer.Write(value);
		}

		public sealed override void WriteTo(SqlWriter writer) {
			WriteToInternal(writer, false);
		}
	}
}
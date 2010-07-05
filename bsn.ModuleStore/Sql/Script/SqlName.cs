using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlName: SqlToken {
		private readonly string value;

		protected SqlName(string name) {
			value = name;
		}

		public string Value {
			get {
				return value;
			}
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write(value);
		}
	}
}
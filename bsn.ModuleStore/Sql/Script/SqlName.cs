using System;
using System.IO;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlName: SqlToken {
		private readonly string value;

		protected SqlName(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			if (string.IsNullOrEmpty("name")) {
				throw new ArgumentException("The name cannot be empty", "name");
			}
			value = name;
		}

		public string Value {
			get {
				return value;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write(value);
		}
	}
}
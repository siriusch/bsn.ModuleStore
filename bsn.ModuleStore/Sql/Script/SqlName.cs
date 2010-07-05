using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlName: SqlToken {
		private readonly string name;

		protected SqlName(string name) {
			this.name = name;
		}

		public string Value {
			get {
				return name;
			}
		}

		public override string ToString() {
			return name;
		}
	}
}
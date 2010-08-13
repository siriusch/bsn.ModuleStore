using System;
using System.IO;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlScriptableToken: SqlToken {
		public override sealed string ToString() {
			using (StringWriter writer = new StringWriter()) {
				WriteTo(new SqlWriter(writer));
				return writer.ToString();
			}
		}

		public abstract void WriteTo(SqlWriter writer);
	}
}
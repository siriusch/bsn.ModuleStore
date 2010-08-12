using System;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlToken: SemanticToken {}

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
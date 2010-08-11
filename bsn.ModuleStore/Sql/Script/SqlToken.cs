using System;
using System.Globalization;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlToken: SemanticToken {
		public override sealed string ToString() {
			IScriptable scriptable = this as IScriptable;
			if (scriptable != null) {
				using (StringWriter writer = new StringWriter()) {
					scriptable.WriteTo(new SqlWriter(writer));
					return writer.ToString();
				}
			}
			return base.ToString();
		}
	}
}
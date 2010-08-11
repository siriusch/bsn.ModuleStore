using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class OpenxmlSchema: SqlToken, IScriptable {
		protected abstract void WriteToInternal(SqlWriter writer);

		public void WriteTo(SqlWriter writer) {
			writer.Write("WITH (");
			WriteToInternal(writer);
			writer.Write(')');
		}
	}
}
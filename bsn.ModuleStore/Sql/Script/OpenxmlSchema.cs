using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class OpenxmlSchema: SqlToken, IScriptable {
		protected abstract void WriteToInternal(TextWriter writer);

		public void WriteTo(TextWriter writer) {
			writer.Write("WITH (");
			WriteToInternal(writer);
			writer.Write(')');
		}
	}
}
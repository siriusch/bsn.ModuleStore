using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class OpenxmlSchema: SqlScriptableToken {
		public override void WriteTo(SqlWriter writer) {
			writer.Write("WITH (");
			WriteToInternal(writer);
			writer.Write(')');
		}

		protected abstract void WriteToInternal(SqlWriter writer);
	}
}
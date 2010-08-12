using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class TypeNameExtended: TypeName {
		protected TypeNameExtended(SqlIdentifier identifier): base(identifier) {}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write('(');
			WriteArguments(writer);
			writer.Write(')');
		}

		protected abstract void WriteArguments(SqlWriter writer);
	}
}
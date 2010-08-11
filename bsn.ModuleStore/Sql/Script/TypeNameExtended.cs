using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class TypeNameExtended: TypeName {
		protected TypeNameExtended(SqlIdentifier identifier): base(identifier) {}

		protected abstract void WriteArguments(TextWriter writer);

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			writer.Write('(');
			WriteArguments(writer);
			writer.Write(')');
		}
	}
}
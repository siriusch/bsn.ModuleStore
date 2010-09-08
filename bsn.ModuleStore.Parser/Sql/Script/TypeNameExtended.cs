using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class TypeNameExtended: TypeName {
		protected TypeNameExtended(SqlIdentifier identifier): base(identifier) {}

		protected internal override void WriteToInternal(SqlWriter writer, bool isPartOfQualifiedName) {
			base.WriteToInternal(writer, isPartOfQualifiedName);
			writer.Write('(');
			WriteArguments(writer);
			writer.Write(')');
		}

		protected abstract void WriteArguments(SqlWriter writer);
	}
}
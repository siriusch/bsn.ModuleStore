using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	internal class AlterOfCreateStatement<T>: Statement where T: CreateStatement, ICreateOrAlterStatement {
		private readonly T owner;

		public AlterOfCreateStatement(T owner) {
			Debug.Assert(owner != null);
			this.owner = owner;
		}

		public override void WriteTo(SqlWriter writer) {
			owner.WriteToInternal(writer, "ALTER");
		}
	}
}
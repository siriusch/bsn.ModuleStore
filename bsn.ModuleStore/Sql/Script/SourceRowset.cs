namespace bsn.ModuleStore.Sql.Script {
	public abstract class SourceRowset: SqlToken {
		private readonly AliasName aliasName;

		protected SourceRowset(AliasName aliasName) {
			this.aliasName = aliasName;
		}
	}
}
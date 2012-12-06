namespace bsn.ModuleStore.Sql.Script {
	public interface ITableBound {
		Qualified<SchemaName, TableName> TableName {
			get;
		}
	}
}
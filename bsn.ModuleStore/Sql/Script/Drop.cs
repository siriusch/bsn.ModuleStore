using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("DROP")]
	[Terminal("DROP_PERSISTED")]
	public class Drop: DdlOperation {}
}
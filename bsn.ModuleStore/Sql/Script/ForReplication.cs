using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("FOR_REPLICATION")]
	public class ForReplication: SqlToken {
		public ForReplication() {}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("FOR REPLICATION");
		}
	}
}
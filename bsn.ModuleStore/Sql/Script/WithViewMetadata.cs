using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("WITH_VIEW_METADATA")]
	public class WithViewMetadata: SqlToken {
		public WithViewMetadata() {}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("WITH VIEW_METADATA");
		}
	}
}
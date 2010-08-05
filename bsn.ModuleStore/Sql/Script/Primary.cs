using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("PRIMARY")]
	public class Primary: SqlToken {
		public Primary() {}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("PRIMARY");
		}
	}
}
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("WITH_RECOMPILE")]
	public class WithRecompile: SqlToken {
		public WithRecompile() {}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("WITH RECOMPILE");
		}
	}
}
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("WITH_CHECK_OPTION")]
	public class WithCheckOption: SqlToken {
		public WithCheckOption() {}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("WITH CHECK OPTION");
		}
	}
}
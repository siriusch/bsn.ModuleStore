using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("OUTPUT")]
	public class Output: SqlToken {
		public Output() {}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("OUTPUT");
		}
	}
}
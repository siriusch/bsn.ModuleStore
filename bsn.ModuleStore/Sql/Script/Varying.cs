using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("VARYING")]
	public class Varying: SqlToken {
		public Varying() {}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("VARYING");
		}
	}
}
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("CALLED_ON_NULL_INPUT")]
	[Terminal("RETURNS_NULL_ON_NULL_INPUT")]
	public class FunctionOption: SqlToken {
		private readonly string value;

		public FunctionOption(string value) {
			this.value = value.ToUpperInvariant();
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write(value);
		}
	}
}
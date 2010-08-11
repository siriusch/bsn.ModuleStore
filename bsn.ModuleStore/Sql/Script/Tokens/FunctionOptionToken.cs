using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class FunctionOptionToken: SqlToken {
		[Rule("<OptionalFunctionOption> ::=")]
		public FunctionOptionToken() {}

		public virtual FunctionOption FunctionOption {
			get {
				return FunctionOption.None;
			}
		}
	}
}
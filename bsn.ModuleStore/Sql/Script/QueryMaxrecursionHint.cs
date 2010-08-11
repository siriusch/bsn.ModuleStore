using System.Globalization;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class QueryMaxrecursionHint: QueryHint {
		private readonly long maxRecursion;

		[Rule("<QueryHint> ::= OPTION_MAXRECURSION IntegerLiteral ')'", ConstructorParameterMapping = new[] {1})]
		public QueryMaxrecursionHint(IntegerLiteral maxRecursion) {
			this.maxRecursion = maxRecursion.Value;
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("OPTION (MAXRECURSION ");
			writer.Write(maxRecursion.ToString(NumberFormatInfo.InvariantInfo));
			writer.Write(')');
		}

		public override bool HasValue {
			get {
				return true;
			}
		}
	}
}
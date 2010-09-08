using System;
using System.Globalization;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class QueryMaxrecursionHint: QueryHint {
		private readonly long maxRecursion;

		[Rule("<QueryHint> ::= OPTION_MAXRECURSION IntegerLiteral ')'", ConstructorParameterMapping = new[] {1})]
		public QueryMaxrecursionHint(IntegerLiteral maxRecursion) {
			this.maxRecursion = maxRecursion.Value;
		}

		public override bool HasValue {
			get {
				return true;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("OPTION (MAXRECURSION ");
			writer.Write(maxRecursion.ToString(NumberFormatInfo.InvariantInfo));
			writer.Write(')');
		}
	}
}
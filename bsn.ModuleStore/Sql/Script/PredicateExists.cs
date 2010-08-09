using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateExists: Predicate {
		private readonly SelectQuery selectQuery;

		[Rule("<PredicateExists> ::= EXISTS '(' <SelectQuery> ')'", ConstructorParameterMapping = new[] {2})]
		public PredicateExists(SelectQuery selectQuery) {
			if (selectQuery == null) {
				throw new ArgumentNullException("selectQuery");
			}
			this.selectQuery = selectQuery;
		}

		public SelectQuery SelectQuery {
			get {
				return selectQuery;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("EXISTS (");
			writer.WriteScript(selectQuery);
			writer.Write(')');
		}
	}
}
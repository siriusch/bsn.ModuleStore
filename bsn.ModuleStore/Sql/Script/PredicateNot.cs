using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateNot: Predicate {
		private readonly Predicate predicate;

		[Rule("<PredicateNot> ::= NOT <PredicateBetween>", ConstructorParameterMapping = new[] {1})]
		public PredicateNot(Predicate predicate) {
			if (predicate == null) {
				throw new ArgumentNullException("predicate");
			}
			this.predicate = predicate;
		}

		public Predicate Predicate {
			get {
				return predicate;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("NOT ");
			writer.WriteScript(predicate);
		}
	}
}
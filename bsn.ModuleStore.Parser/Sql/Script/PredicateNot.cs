using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateNot: Predicate {
		private readonly Predicate predicate;

		[Rule("<PredicateNot> ::= ~NOT <PredicateBetween>")]
		public PredicateNot(Predicate predicate) {
			Debug.Assert(predicate != null);
			this.predicate = predicate;
		}

		public Predicate Predicate {
			get {
				return predicate;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("NOT ");
			writer.WriteScript(predicate, WhitespacePadding.None);
		}
	}
}
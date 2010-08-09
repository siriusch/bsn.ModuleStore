using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class PredicateNegable: Predicate {
		private readonly bool not;

		protected PredicateNegable(bool not) {
			this.not = not;
		}

		public bool Not {
			get {
				return not;
			}
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			if (not) {
				writer.Write(" NOT");
			}
		}
	}
}
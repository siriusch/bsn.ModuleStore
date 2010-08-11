using System;
using System.IO;

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

		public override void WriteTo(SqlWriter writer) {
			if (not) {
				writer.Write(" NOT");
			}
		}
	}
}
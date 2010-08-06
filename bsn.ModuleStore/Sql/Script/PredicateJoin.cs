using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class PredicateJoin: Join {
		private readonly Predicate predicate;

		protected PredicateJoin(SourceRowset joinRowset, Predicate predicate): base(joinRowset) {
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
	}
}
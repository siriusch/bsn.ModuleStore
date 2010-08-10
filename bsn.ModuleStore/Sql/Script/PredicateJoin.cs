using System;
using System.IO;

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

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			writer.Write(" ON ");
			writer.WriteScript(predicate);
		}
	}
}
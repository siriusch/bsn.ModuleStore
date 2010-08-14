using System;
using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class PredicateJoin: Join {
		private readonly Predicate predicate;

		protected PredicateJoin(Source joinSource, Predicate predicate): base(joinSource) {
			Debug.Assert(predicate != null);
			this.predicate = predicate;
		}

		public Predicate Predicate {
			get {
				return predicate;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write(" ON ");
			writer.WriteScript(predicate, WhitespacePadding.None);
		}
	}
}
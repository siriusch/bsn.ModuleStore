using System;
using System.Diagnostics;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class PredicateJoin: Join {
		private readonly Predicate predicate;

		protected PredicateJoin(SourceRowset joinRowset, Predicate predicate): base(joinRowset) {
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
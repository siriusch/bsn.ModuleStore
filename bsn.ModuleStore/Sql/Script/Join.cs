using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class Join: SqlToken, IScriptable {
		private readonly SourceRowset joinRowset;

		protected Join(SourceRowset joinRowset): base() {
			if (joinRowset == null) {
				throw new ArgumentNullException("joinRowset");
			}
			this.joinRowset = joinRowset;
		}

		public SourceRowset JoinRowset {
			get {
				return joinRowset;
			}
		}

		public abstract JoinKind Kind {
			get;
		}

		public virtual void WriteTo(TextWriter writer) {
			writer.Write("JOIN ");
			writer.WriteScript(joinRowset);
		}
	}
}
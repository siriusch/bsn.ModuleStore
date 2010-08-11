using System;
using System.Diagnostics;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class Join: SqlToken, IScriptable {
		private readonly SourceRowset joinRowset;

		protected Join(SourceRowset joinRowset): base() {
			Debug.Assert(joinRowset != null);
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

		public virtual void WriteTo(SqlWriter writer) {
			writer.Write("JOIN ");
			writer.WriteScript(joinRowset, WhitespacePadding.None);
		}
	}
}
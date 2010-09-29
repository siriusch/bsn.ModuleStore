using System;
using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class Join: SqlScriptableToken {
		private readonly Source joinSource;

		protected Join(Source joinSource): base() {
			Debug.Assert(joinSource != null);
			this.joinSource = joinSource;
		}

		public Source JoinSource {
			get {
				return joinSource;
			}
		}

		public abstract JoinKind Kind {
			get;
		}

		protected abstract string JoinSpecifier {
			get;
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write(JoinSpecifier);
			writer.Write(' ');
			writer.WriteScript(joinSource, WhitespacePadding.None);
		}
	}
}
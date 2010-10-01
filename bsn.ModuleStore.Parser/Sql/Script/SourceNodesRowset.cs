using System;
using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SourceNodesRowset: SourceRowset {
		private readonly NamedFunction functionCall;

		protected SourceNodesRowset(NamedFunction functionCall, RowsetAlias rowsetAlias): base(rowsetAlias) {
			Debug.Assert(functionCall != null);
			functionCall.FunctionName.LockOverride();
			this.functionCall = functionCall;
		}

		public NamedFunction FunctionCall {
			get {
				return functionCall;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(functionCall, WhitespacePadding.None);
			base.WriteTo(writer);
		}
	}
}
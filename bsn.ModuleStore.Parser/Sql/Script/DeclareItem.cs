using System;
using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class DeclareItem: SqlScriptableToken {
		private readonly VariableName variableName;

		protected DeclareItem(VariableName variableName): base() {
			Debug.Assert(variableName != null);
			this.variableName = variableName;
		}

		public VariableName VariableName {
			get {
				return variableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(variableName, WhitespacePadding.SpaceAfter);
		}
	}
}
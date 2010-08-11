using System;
using System.Diagnostics;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class DeclareItem: SqlToken, IScriptable {
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

		public virtual void WriteTo(TextWriter writer) {
			writer.WriteScript(variableName);
			writer.Write(' ');
		}
	}
}
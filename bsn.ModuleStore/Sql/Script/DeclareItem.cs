using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class DeclareItem: SqlToken, IScriptable {
		private readonly VariableName variableName;

		protected DeclareItem(VariableName variableName): base() {
			if (variableName == null) {
				throw new ArgumentNullException("variableName");
			}
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
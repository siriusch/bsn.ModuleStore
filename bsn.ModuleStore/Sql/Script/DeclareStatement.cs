using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class DeclareStatement: Statement {
		private readonly VariableName variableName;

		protected DeclareStatement(VariableName variableName): base() {
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
	}
}
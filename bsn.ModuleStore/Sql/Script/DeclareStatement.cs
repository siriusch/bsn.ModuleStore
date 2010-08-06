using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class DeclareStatement: SqlStatement {
		private readonly VariableName variable;

		protected DeclareStatement(VariableName variable): base() {
			if (variable == null) {
				throw new ArgumentNullException("variable");
			}
			this.variable = variable;
		}

		public VariableName Variable {
			get {
				return variable;
			}
		}
	}
}
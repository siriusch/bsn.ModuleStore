using System;
using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SetVariableStatement: Statement {
		private readonly VariableName variableName;

		public SetVariableStatement(VariableName variableName) {
			this.variableName = variableName;
			Debug.Assert(variableName != null);
		}

		public VariableName VariableName {
			get {
				return variableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("SET ");
			writer.WriteScript(variableName, WhitespacePadding.None);
			writer.Write('=');
		}
	}
}
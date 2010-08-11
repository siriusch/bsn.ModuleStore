using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SetVariableStatement: Statement {
		private readonly VariableAssignment variableAssignment;

		[Rule("<SetVariableStatement> ::= SET <VariableAssignment>", ConstructorParameterMapping = new[] {1})]
		public SetVariableStatement(VariableAssignment variableAssignment) {
			Debug.Assert(variableAssignment != null);
			this.variableAssignment = variableAssignment;
		}

		public VariableAssignment VariableAssignment {
			get {
				return variableAssignment;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("SET ");
			writer.WriteScript(variableAssignment);
		}
	}
}
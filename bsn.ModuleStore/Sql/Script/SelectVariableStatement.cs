using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class SelectVariableStatement: Statement {
		private readonly List<VariableAssignment> variableAssignments;

		[Rule("<SelectVariableStatement> ::= SELECT <VariableAssignmentList>", ConstructorParameterMapping=new[] { 1 })]
		public SelectVariableStatement(Sequence<VariableAssignment> variableAssignments) {
			this.variableAssignments = variableAssignments.ToList();
		}

		public List<VariableAssignment> VariableAssignments {
			get {
				return variableAssignments;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("SELECT ");
			writer.WriteSequence(variableAssignments, null, ", ", null);
		}
	}
}

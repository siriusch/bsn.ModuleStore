using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DeclareTableStatement: DeclareStatement {
		private readonly VariableName variableName;
		private readonly List<TableDefinition> tableDefinitions;

		[Rule("<DeclareStatement> ::= DECLARE <VariableName> <OptionalAs> TABLE <TableDefinitionGroup>", ConstructorParameterMapping = new[] {1, 4})]
		public DeclareTableStatement(VariableName variableName, Sequence<TableDefinition> tableDefinitions): base() {
			Debug.Assert(variableName != null);
			this.variableName = variableName;
			this.tableDefinitions = tableDefinitions.ToList();
		}

		public VariableName VariableName {
			get {
				return variableName;
			}
		}

		public List<TableDefinition> TableDefinitions {
			get {
				return tableDefinitions;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("DECLARE ");
			writer.WriteScript(variableName);
			writer.WriteLine(" TABLE (");
			writer.WriteSequence(tableDefinitions, "\t", ",", Environment.NewLine);
			writer.WriteLine(")");
		}
	}
}
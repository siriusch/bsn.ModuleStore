using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DeclareTableStatement: DeclareStatement {
		private readonly List<TableDefinition> tableDefinitions;
		private readonly VariableName variableName;

		[Rule("<DeclareStatement> ::= ~DECLARE <VariableName> ~<OptionalAs> ~TABLE <TableDefinitionGroup>")]
		public DeclareTableStatement(VariableName variableName, Sequence<TableDefinition> tableDefinitions): base() {
			Debug.Assert(variableName != null);
			this.variableName = variableName;
			this.tableDefinitions = tableDefinitions.ToList();
		}

		public IEnumerable<TableDefinition> TableDefinitions {
			get {
				return tableDefinitions;
			}
		}

		public VariableName VariableName {
			get {
				return variableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("DECLARE ");
			writer.WriteScript(variableName, WhitespacePadding.None);
			writer.Write(" TABLE (");
			writer.IncreaseIndent();
			writer.WriteScriptSequence(tableDefinitions, WhitespacePadding.NewlineBefore, ",");
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(')');
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateFunctionTableStatement: CreateFunctionStatement<StatementBlock> {
		private readonly VariableName resultVariableName;
		private readonly List<TableDefinition> tableDefinitions;

		[Rule("<CreateFunctionStatement> ::= CREATE FUNCTION <FunctionName> '(' <OptionalFunctionParameterList> _RETURNS <VariableName> TABLE <TableDefinitionGroup> <OptionalFunctionOption> <OptionalAs> <StatementBlock>", ConstructorParameterMapping = new[] {2, 4, 6, 8, 9, 11})]
		public CreateFunctionTableStatement(FunctionName functionName, Optional<Sequence<FunctionParameter>> parameters, VariableName resultVariableName, Sequence<TableDefinition> tableDefinitions, Optional<FunctionOption> options, StatementBlock body): base(functionName, parameters, options, body) {
			if (resultVariableName == null) {
				throw new ArgumentNullException("resultVariableName");
			}
			if (tableDefinitions == null) {
				throw new ArgumentNullException("tableDefinitions");
			}
			this.resultVariableName = resultVariableName;
			this.tableDefinitions = tableDefinitions.ToList();
		}

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			resultVariableName.WriteTo(writer);
			writer.Write(" TABLE ");
			tableDefinitions.WriteTo(writer);
			WriteOptions(writer);
			writer.WriteLine(" AS (");
			writer.WriteSequence(tableDefinitions, "\t", ";", Environment.NewLine);
			writer.WriteLine(")");
		}
	}
}
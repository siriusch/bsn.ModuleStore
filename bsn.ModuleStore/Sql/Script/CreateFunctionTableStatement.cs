using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateFunctionTableStatement: CreateFunctionStatement<StatementBlock> {
		private readonly VariableName resultVariableName;
		private readonly TableDefinition tableDefinition;

		[Rule("<CreateFunctionStatement> ::= CREATE FUNCTION <FunctionName> '(' <OptionalFunctionParameterList> _RETURNS <VariableName> TABLE <TableDefinitionGroup> <OptionalFunctionOption> <OptionalAs> <StatementBlock>", ConstructorParameterMapping=new[] { 2, 4, 6, 8, 9, 11 })]
		public CreateFunctionTableStatement(FunctionName functionName, Optional<Sequence<FunctionParameter>> parameters, VariableName resultVariableName, TableDefinition tableDefinition, Optional<FunctionOption> options, StatementBlock body)
				: base(functionName, parameters, options, body) {
			if (resultVariableName == null) {
				throw new ArgumentNullException("resultVariableName");
			}
			if (tableDefinition == null) {
				throw new ArgumentNullException("tableDefinition");
			}
			this.resultVariableName = resultVariableName;
			this.tableDefinition = tableDefinition;
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			base.WriteTo(writer);
			resultVariableName.WriteTo(writer);
			writer.Write(" TABLE ");
			tableDefinition.WriteTo(writer);
			WriteOptions(writer);
			writer.Write(" AS ");
			Body.WriteTo(writer);
		}
	}
}
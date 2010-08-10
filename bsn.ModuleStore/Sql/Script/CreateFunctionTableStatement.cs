using System;
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateFunctionTableStatement: CreateFunctionStatement<StatementBlock> {
		private readonly VariableName resultVariableName;
		private readonly List<TableDefinition> tableDefinitions;

		[Rule("<CreateFunctionStatement> ::= CREATE FUNCTION <FunctionName> '(' <OptionalFunctionParameterList> _RETURNS <VariableName> TABLE <TableDefinitionGroup> <OptionalFunctionOption> <OptionalAs> <StatementBlock>", ConstructorParameterMapping = new[] {2, 4, 6, 8, 9, 11})]
		public CreateFunctionTableStatement(FunctionName functionName, Optional<Sequence<FunctionParameter>> parameters, VariableName resultVariableName, Sequence<TableDefinition> tableDefinitions, FunctionOptionToken options, StatementBlock body): base(functionName, parameters, options, body) {
			if (resultVariableName == null) {
				throw new ArgumentNullException("resultVariableName");
			}
			if (tableDefinitions == null) {
				throw new ArgumentNullException("tableDefinitions");
			}
			this.resultVariableName = resultVariableName;
			this.tableDefinitions = tableDefinitions.ToList();
		}

		public VariableName ResultVariableName {
			get {
				return resultVariableName;
			}
		}

		public List<TableDefinition> TableDefinitions {
			get {
				return tableDefinitions;
			}
		}

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			writer.WriteScript(resultVariableName);
			writer.WriteLine(" TABLE (");
			writer.WriteSequence(tableDefinitions, "\t", ";", Environment.NewLine);
			writer.Write(')');
			writer.WriteValue(Option, " ", null);
			writer.WriteLine(" AS (");
			writer.WriteScript(Body);
			writer.Write(")");
		}
	}
}
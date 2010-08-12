using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateFunctionTableStatement: CreateFunctionStatement<StatementBlock> {
		private readonly VariableName resultVariableName;
		private readonly List<TableDefinition> tableDefinitions;

		[Rule("<CreateFunctionStatement> ::= CREATE FUNCTION <FunctionName> '(' <OptionalFunctionParameterList> _RETURNS <VariableName> TABLE <TableDefinitionGroup> <OptionalFunctionOption> <OptionalAs> <StatementBlock>", ConstructorParameterMapping = new[] {2, 4, 6, 8, 9, 11})]
		public CreateFunctionTableStatement(FunctionName functionName, Optional<Sequence<FunctionParameter>> parameters, VariableName resultVariableName, Sequence<TableDefinition> tableDefinitions, FunctionOptionToken options, StatementBlock body): base(functionName, parameters, options, body) {
			Debug.Assert(resultVariableName != null);
			Debug.Assert(tableDefinitions != null);
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

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteScript(resultVariableName, WhitespacePadding.None);
			writer.Write(" TABLE (");
			writer.IncreaseIndent();
			writer.WriteSequence(tableDefinitions, WhitespacePadding.NewlineBefore, ";");
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(')');
			writer.WriteEnum(Option, WhitespacePadding.SpaceBefore);
			writer.WriteLine();
			writer.Write("AS");
			writer.IncreaseIndent();
			writer.WriteScript(Body, WhitespacePadding.NewlineBefore);
			writer.DecreaseIndent();
		}
	}
}
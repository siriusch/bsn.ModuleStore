using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateFunctionTableStatement<T>: CreateFunctionStatement<T> where T: SqlScriptableToken {
		private readonly VariableName resultVariableName;
		private readonly List<TableDefinition> tableDefinitions;

		[Rule("<CreateFunctionStatement> ::= ~CREATE ~FUNCTION <FunctionNameQualified> ~'(' <OptionalFunctionParameterList> ~_RETURNS ~TABLE <TableDefinitionGroup> <OptionalFunctionOption> ~<OptionalAs> <ExternalName>", typeof(ExternalName))]
		public CreateFunctionTableStatement(Qualified<SchemaName, FunctionName> functionName, Optional<Sequence<FunctionParameter>> parameters, Sequence<TableDefinition> tableDefinitions, FunctionOptionToken options, T body): this(functionName, parameters, null, tableDefinitions, options, body) {}

		[Rule("<CreateFunctionStatement> ::= ~CREATE ~FUNCTION <FunctionNameQualified> ~'(' <OptionalFunctionParameterList> ~_RETURNS <VariableName> ~TABLE <TableDefinitionGroup> <OptionalFunctionOption> ~<OptionalAs> <StatementBlock>", typeof(StatementBlock))]
		public CreateFunctionTableStatement(Qualified<SchemaName, FunctionName> functionName, Optional<Sequence<FunctionParameter>> parameters, VariableName resultVariableName, Sequence<TableDefinition> tableDefinitions, FunctionOptionToken options, T body)
				: base(functionName, parameters, options, body) {
			Debug.Assert(tableDefinitions != null);
			this.resultVariableName = resultVariableName;
			this.tableDefinitions = tableDefinitions.ToList();
		}

		public VariableName ResultVariableName {
			get {
				return resultVariableName;
			}
		}

		public IEnumerable<TableDefinition> TableDefinitions {
			get {
				return tableDefinitions;
			}
		}

		protected override void WriteToInternal(SqlWriter writer, string command) {
			base.WriteToInternal(writer, command);
			writer.WriteScript(resultVariableName, WhitespacePadding.SpaceAfter);
			writer.Write("TABLE (");
			writer.IncreaseIndent();
			writer.WriteScriptSequence(tableDefinitions, WhitespacePadding.NewlineBefore, ";");
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

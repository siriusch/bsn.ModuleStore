using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateFunctionInlineStatement: CreateFunctionStatement<SelectStatement> {
		[Rule("<CreateFunctionStatement> ::= ~CREATE ~FUNCTION <FunctionNameQualified> ~'(' <OptionalFunctionParameterList> ~')' ~RETURNS ~TABLE <OptionalFunctionOption> ~<OptionalAs> ~RETURN <FunctionInlineSelect>")]
		public CreateFunctionInlineStatement(Qualified<SchemaName, FunctionName> functionName, Optional<Sequence<FunctionParameter>> parameters, OptionToken option, SelectStatement body): base(functionName, parameters, option, body) {}

		protected override void WriteToInternal(SqlWriter writer, string command) {
			base.WriteToInternal(writer, command);
			writer.Write("TABLE");
			writer.WriteScript(Option, WhitespacePadding.SpaceBefore);
			writer.WriteLine();
			writer.Write("AS RETURN (");
			writer.IncreaseIndent();
			writer.WriteScript(Body, WhitespacePadding.NewlineBefore);
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(')');
		}
	}
}
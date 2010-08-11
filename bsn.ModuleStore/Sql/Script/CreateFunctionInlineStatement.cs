using System;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateFunctionInlineStatement: CreateFunctionStatement<SelectStatement> {
		[Rule("<CreateFunctionStatement> ::= CREATE FUNCTION <FunctionName> '(' <OptionalFunctionParameterList> _RETURNS TABLE <OptionalFunctionOption> <OptionalAs> RETURN <FunctionInlineSelect>", ConstructorParameterMapping = new[] {2, 4, 7, 10})]
		public CreateFunctionInlineStatement(FunctionName functionName, Optional<Sequence<FunctionParameter>> parameters, FunctionOptionToken options, SelectStatement body): base(functionName, parameters, options, body) {}

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			writer.Write("TABLE");
			writer.WriteValue(Option, " ", null);
			writer.WriteLine(" AS RETURN (");
			writer.WriteLine(Body);
			writer.Write(')');
		}
	}
}
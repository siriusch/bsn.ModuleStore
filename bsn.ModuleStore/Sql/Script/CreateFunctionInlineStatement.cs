using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateFunctionInlineStatement: CreateFunctionStatement<SelectStatement> {
		[Rule("<CreateFunctionStatement> ::= CREATE FUNCTION <FunctionName> '(' <OptionalFunctionParameterList> _RETURNS TABLE <OptionalFunctionOption> <OptionalAs> RETURN <FunctionInlineSelect>", ConstructorParameterMapping = new[] {2, 4, 7, 10})]
		public CreateFunctionInlineStatement(FunctionName functionName, Optional<Sequence<FunctionParameter>> parameters, Optional<FunctionOption> options, SelectStatement body): base(functionName, parameters, options, body) {}

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			writer.Write("TABLE");
			WriteOptions(writer);
			writer.Write(" AS RETURN (");
			Body.WriteTo(writer);
			writer.Write(')');
		}
	}
}
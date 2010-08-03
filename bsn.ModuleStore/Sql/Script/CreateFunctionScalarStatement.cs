using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateFunctionScalarStatement: CreateFunctionStatement<StatementBlock> {
		private readonly TypeName returnTypeName;

		[Rule("<CreateFunctionStatement> ::= CREATE FUNCTION <FunctionName> '(' <OptionalFunctionParameterList> _RETURNS <TypeName> <OptionalFunctionOption> <OptionalAs> <StatementBlock>", ConstructorParameterMapping = new[] {2, 4, 6, 7, 9})]
		public CreateFunctionScalarStatement(FunctionName functionName, Optional<Sequence<FunctionParameter>> parameters, TypeName returnTypeName, Optional<FunctionOption> options, StatementBlock body): base(functionName, parameters, options, body) {
			if (returnTypeName == null) {
				throw new ArgumentNullException("returnTypeName");
			}
			this.returnTypeName = returnTypeName;
		}

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			returnTypeName.WriteTo(writer);
			WriteOptions(writer);
			writer.Write(" AS ");
			Body.WriteTo(writer);
		}
	}
}
using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateFunctionScalarStatement: CreateFunctionStatement<StatementBlock> {
		private readonly TypeName returnTypeName;

		[Rule("<CreateFunctionStatement> ::= CREATE FUNCTION <FunctionName> '(' <OptionalFunctionParameterList> _RETURNS <TypeName> <OptionalFunctionOption> <OptionalAs> <StatementBlock>", ConstructorParameterMapping = new[] {2, 4, 6, 7, 9})]
		public CreateFunctionScalarStatement(FunctionName functionName, Optional<Sequence<FunctionParameter>> parameters, TypeName returnTypeName, FunctionOptionToken options, StatementBlock body): base(functionName, parameters, options, body) {
			Debug.Assert(returnTypeName != null);
			this.returnTypeName = returnTypeName;
		}

		public TypeName ReturnTypeName {
			get {
				return returnTypeName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteScript(returnTypeName, WhitespacePadding.None);
			writer.WriteEnum(Option, WhitespacePadding.SpaceBefore);
			writer.WriteLine();
			writer.Write("AS");
			writer.IncreaseIndent();
			writer.WriteScript(Body, WhitespacePadding.NewlineBefore);
			writer.DecreaseIndent();
		}
	}
}
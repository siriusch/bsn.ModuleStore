using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateFunctionScalarStatement<T>: CreateFunctionStatement<T> where T: SqlScriptableToken {
		private readonly Qualified<SchemaName, TypeName> returnTypeName;

		[Rule("<CreateFunctionStatement> ::= ~CREATE ~FUNCTION <FunctionNameQualified> ~'(' <OptionalFunctionParameterList> ~')' ~RETURNS <TypeNameQualified> <OptionalFunctionOption> ~<OptionalAs> <StatementBlock>", typeof(StatementBlock))]
		[Rule("<CreateFunctionStatement> ::= ~CREATE ~FUNCTION <FunctionNameQualified> ~'(' <OptionalFunctionParameterList> ~')' ~RETURNS <TypeNameQualified> <OptionalFunctionOption> ~<OptionalAs> <ExternalName>", typeof(ExternalName))]
		public CreateFunctionScalarStatement(Qualified<SchemaName, FunctionName> functionName, Optional<Sequence<FunctionParameter>> parameters, Qualified<SchemaName, TypeName> returnTypeName, OptionToken option, T body): base(functionName, parameters, option, body) {
			Debug.Assert(returnTypeName != null);
			this.returnTypeName = returnTypeName;
		}

		public Qualified<SchemaName, TypeName> ReturnTypeName {
			get {
				return returnTypeName;
			}
		}

		protected override void WriteToInternal(SqlWriter writer, string command) {
			base.WriteToInternal(writer, command);
			writer.WriteScript(returnTypeName, WhitespacePadding.None);
			writer.WriteScript(Option, WhitespacePadding.SpaceBefore);
			writer.WriteLine();
			writer.Write("AS");
			writer.IncreaseIndent();
			writer.WriteScript(Body, WhitespacePadding.NewlineBefore);
			writer.DecreaseIndent();
		}
	}
}
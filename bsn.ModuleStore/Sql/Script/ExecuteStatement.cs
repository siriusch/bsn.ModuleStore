using System;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class ExecuteStatement: SqlStatement {
		[Rule("<ExecuteStatement> ::= EXECUTE <VariableName> '=' <ProcedureNameQualified> <ExecuteParameterGroup> <ProcedureOptionGroup>", ConstructorParameterMapping = new[] {1, 3, 4, 5})]
		public ExecuteStatement(VariableName resultVariable, Qualified<ProcedureName> procedureName, Optional<Sequence<ExecuteParameter>> parameters, Optional<WithRecompileToken> recompile) {}

		[Rule("<ExecuteStatement> ::= EXECUTE <ProcedureNameQualified> <ExecuteParameterGroup> <ProcedureOptionGroup>", ConstructorParameterMapping = new[] {1, 2, 3})]
		public ExecuteStatement(Qualified<ProcedureName> procedureName, Optional<Sequence<ExecuteParameter>> parameters, Optional<WithRecompileToken> recompile): this(null, procedureName, parameters, recompile) {}
	}
}
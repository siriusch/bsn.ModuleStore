using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExecuteStatement: Statement {
		private readonly List<ExecuteParameter> parameters;
		private readonly Qualified<SchemaName, ProcedureName> procedureName;
		private readonly bool recompile;
		private readonly VariableName resultVariableName;

		[Rule("<ExecuteStatement> ::= EXECUTE <VariableName> '=' <ProcedureNameQualified> <ExecuteParameterGroup> <ProcedureOptionGroup>", ConstructorParameterMapping = new[] {1, 3, 4, 5})]
		public ExecuteStatement(VariableName resultVariableName, Qualified<SchemaName, ProcedureName> procedureName, Optional<Sequence<ExecuteParameter>> parameters, Optional<WithRecompileToken> recompile) {
			Debug.Assert(procedureName != null);
			this.resultVariableName = resultVariableName;
			this.procedureName = procedureName;
			this.parameters = parameters.ToList();
			this.recompile = recompile.HasValue();
		}

		[Rule("<ExecuteStatement> ::= EXECUTE <ProcedureNameQualified> <ExecuteParameterGroup> <ProcedureOptionGroup>", ConstructorParameterMapping = new[] {1, 2, 3})]
		public ExecuteStatement(Qualified<SchemaName, ProcedureName> procedureName, Optional<Sequence<ExecuteParameter>> parameters, Optional<WithRecompileToken> recompile): this(null, procedureName, parameters, recompile) {}

		public IEnumerable<ExecuteParameter> Parameters {
			get {
				return parameters;
			}
		}

		public Qualified<SchemaName, ProcedureName> ProcedureName {
			get {
				return procedureName;
			}
		}

		public bool Recompile {
			get {
				return recompile;
			}
		}

		public VariableName ResultVariableName {
			get {
				return resultVariableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("EXEC ");
			writer.WriteScript(resultVariableName, WhitespacePadding.None, null, "=");
			writer.WriteScript(procedureName, WhitespacePadding.None);
			writer.WriteScriptSequence(parameters, WhitespacePadding.SpaceBefore, null);
			if (recompile) {
				writer.Write(" WITH RECOMPILE");
			}
		}
	}
}
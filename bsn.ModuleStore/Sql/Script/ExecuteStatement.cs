using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExecuteStatement: Statement {
		private readonly List<ExecuteParameter> parameters;
		private readonly Qualified<ProcedureName> procedureName;
		private readonly bool recompile;
		private readonly VariableName resultVariableName;

		[Rule("<ExecuteStatement> ::= EXECUTE <VariableName> '=' <ProcedureNameQualified> <ExecuteParameterGroup> <ProcedureOptionGroup>", ConstructorParameterMapping = new[] {1, 3, 4, 5})]
		public ExecuteStatement(VariableName resultVariableName, Qualified<ProcedureName> procedureName, Optional<Sequence<ExecuteParameter>> parameters, Optional<WithRecompileToken> recompile) {
			if (procedureName == null) {
				throw new ArgumentNullException("procedureName");
			}
			this.resultVariableName = resultVariableName;
			this.procedureName = procedureName;
			this.parameters = parameters.ToList();
			this.recompile = recompile.HasValue();
		}

		[Rule("<ExecuteStatement> ::= EXECUTE <ProcedureNameQualified> <ExecuteParameterGroup> <ProcedureOptionGroup>", ConstructorParameterMapping = new[] {1, 2, 3})]
		public ExecuteStatement(Qualified<ProcedureName> procedureName, Optional<Sequence<ExecuteParameter>> parameters, Optional<WithRecompileToken> recompile): this(null, procedureName, parameters, recompile) {}

		public List<ExecuteParameter> Parameters {
			get {
				return parameters;
			}
		}

		public Qualified<ProcedureName> ProcedureName {
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

		public override void WriteTo(TextWriter writer) {
			writer.Write("EXEC ");
			writer.WriteScript(resultVariableName, null, "=");
			writer.WriteScript(procedureName);
			writer.WriteSequence(parameters, " ", null, null);
			writer.WriteWithRecompile(recompile, " ", null);
		}
	}
}
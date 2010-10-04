using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExecuteStatement: Statement {
		private readonly OptionToken option;
		private readonly List<ExecuteParameter> parameters;
		private readonly Qualified<SchemaName, ProcedureName> procedureName;
		private readonly VariableName resultVariableName;

		[Rule("<ExecuteStatement> ::= ~EXECUTE <VariableName> ~'=' <ProcedureNameQualified> <ExecuteParameterGroup> <ProcedureOptionGroup>")]
		public ExecuteStatement(VariableName resultVariableName, Qualified<SchemaName, ProcedureName> procedureName, Optional<Sequence<ExecuteParameter>> parameters, OptionToken option) {
			Debug.Assert(procedureName != null);
			this.resultVariableName = resultVariableName;
			this.procedureName = procedureName;
			this.parameters = parameters.ToList();
			this.option = option;
		}

		[Rule("<ExecuteStatement> ::= ~EXECUTE <ProcedureNameQualified> <ExecuteParameterGroup> <ProcedureOptionGroup>")]
		public ExecuteStatement(Qualified<SchemaName, ProcedureName> procedureName, Optional<Sequence<ExecuteParameter>> parameters, OptionToken option): this(null, procedureName, parameters, option) {}

		public OptionToken Option {
			get {
				return option;
			}
		}

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

		public VariableName ResultVariableName {
			get {
				return resultVariableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("EXEC ");
			writer.WriteScript(resultVariableName, WhitespacePadding.None, null, "=");
			writer.WriteScript(procedureName, WhitespacePadding.None);
			writer.WriteScriptSequence(parameters, WhitespacePadding.SpaceBefore, ", ");
			writer.WriteScript(option, WhitespacePadding.SpaceBefore);
		}
	}
}
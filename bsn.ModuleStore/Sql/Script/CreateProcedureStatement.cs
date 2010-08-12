using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateProcedureStatement: CreateStatement {
		private readonly StatementBlock body;
		private readonly bool forReplication;
		private readonly List<ProcedureParameter> parameters;
		private readonly ProcedureName procedureName;
		private readonly bool recompile;

		[Rule("<CreateProcedureStatement> ::= CREATE PROCEDURE <ProcedureName> <ProcedureParameterGroup> <ProcedureOptionGroup> <ProcedureFor> AS <StatementBlock>", ConstructorParameterMapping = new[] {2, 3, 4, 5, 7})]
		public CreateProcedureStatement(ProcedureName procedureName, Optional<Sequence<ProcedureParameter>> parameters, Optional<WithRecompileToken> recompile, Optional<ForReplicationToken> forReplication, StatementBlock body) {
			Debug.Assert(procedureName != null);
			Debug.Assert(body != null);
			this.procedureName = procedureName;
			this.parameters = parameters.ToList();
			this.recompile = recompile.HasValue();
			this.forReplication = forReplication.HasValue();
			this.body = body;
		}

		public StatementBlock Body {
			get {
				return body;
			}
		}

		public bool ForReplication {
			get {
				return forReplication;
			}
		}

		public List<ProcedureParameter> Parameters {
			get {
				return parameters;
			}
		}

		public ProcedureName ProcedureName {
			get {
				return procedureName;
			}
		}

		public bool Recompile {
			get {
				return recompile;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CREATE PROCEDURE ");
			writer.WriteScript(procedureName, WhitespacePadding.None);
			writer.IncreaseIndent();
			writer.WriteSequence(parameters, WhitespacePadding.NewlineBefore, ",");
			if (recompile) {
				writer.WriteLine("WITH RECOMPILE");
			}
			if (forReplication) {
				writer.WriteLine("FOR REPLICATION");
			}
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write("AS");
			writer.IncreaseIndent();
			writer.WriteScript(body, WhitespacePadding.NewlineBefore);
			writer.DecreaseIndent();
		}
	}
}
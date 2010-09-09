using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateProcedureStatement: CreateStatement, ICreateOrAlterStatement {
		private readonly StatementBlock body;
		private readonly bool forReplication;
		private readonly List<ProcedureParameter> parameters;
		private readonly Qualified<SchemaName, ProcedureName> procedureName;
		private readonly bool recompile;

		[Rule("<CreateProcedureStatement> ::= CREATE PROCEDURE <ProcedureNameQualified> <ProcedureParameterGroup> <ProcedureOptionGroup> <ProcedureFor> AS <StatementBlock>", ConstructorParameterMapping = new[] {2, 3, 4, 5, 7})]
		public CreateProcedureStatement(Qualified<SchemaName, ProcedureName> procedureName, Optional<Sequence<ProcedureParameter>> parameters, Optional<WithRecompileToken> recompile, Optional<ForReplicationToken> forReplication, StatementBlock body) {
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

		public override ObjectCategory ObjectCategory {
			get {
				return ObjectCategory.Procedure;
			}
		}

		public override string ObjectName {
			get {
				return procedureName.Name.Value;
			}
		}

		public ReadOnlyCollection<ProcedureParameter> Parameters {
			get {
				return parameters.AsReadOnly();
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

		public override Statement CreateAlterStatement() {
			return new AlterOfCreateStatement<CreateProcedureStatement>(this);
		}

		public override DropStatement CreateDropStatement() {
			return new DropProcedureStatement(procedureName);
		}

		public override void WriteTo(SqlWriter writer) {
			WriteToInternal(writer, "CREATE");
		}

		void ICreateOrAlterStatement.WriteToInternal(SqlWriter writer, string command) {
			if (string.IsNullOrEmpty(command)) {
				throw new ArgumentNullException("command");
			}
			WriteToInternal(writer, command);
		}

		private void WriteToInternal(SqlWriter writer, string command) {
			writer.Write(command);
			writer.Write(" PROCEDURE ");
			writer.WriteScript(procedureName, WhitespacePadding.None);
			writer.IncreaseIndent();
			writer.WriteScriptSequence(parameters, WhitespacePadding.NewlineBefore, ",");
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

		protected override string GetObjectSchema() {
			return procedureName.IsQualified ? procedureName.Qualification.Value : string.Empty;
		}
	}
}

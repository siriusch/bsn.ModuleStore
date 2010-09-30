using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateProcedureStatement: CreateStatement, ICreateOrAlterStatement {
		private readonly StatementBlock body;
		private readonly OptionToken option;
		private readonly List<ProcedureParameter> parameters;
		private readonly Qualified<SchemaName, ProcedureName> procedureName;
		private readonly ReplicationToken replication;

		[Rule("<CreateProcedureStatement> ::= ~CREATE ~PROCEDURE <ProcedureNameQualified> <ProcedureParameterGroup> <ProcedureOptionGroup> <ProcedureFor> ~AS <StatementBlock>")]
		public CreateProcedureStatement(Qualified<SchemaName, ProcedureName> procedureName, Optional<Sequence<ProcedureParameter>> parameters, OptionToken option, ReplicationToken replication, StatementBlock body) {
			Debug.Assert(procedureName != null);
			Debug.Assert(body != null);
			this.procedureName = procedureName;
			this.option = option;
			this.replication = replication;
			this.parameters = parameters.ToList();
			this.body = body;
		}

		public StatementBlock Body {
			get {
				return body;
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

		public OptionToken Option {
			get {
				return option;
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

		public ReplicationToken Replication {
			get {
				return replication;
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

		protected override string GetObjectSchema() {
			return procedureName.IsQualified ? procedureName.Qualification.Value : string.Empty;
		}

		private void WriteToInternal(SqlWriter writer, string command) {
			WriteCommentsTo(writer);
			writer.Write(command);
			writer.Write(" PROCEDURE ");
			writer.WriteScript(procedureName, WhitespacePadding.None);
			writer.IncreaseIndent();
			writer.WriteScriptSequence(parameters, WhitespacePadding.NewlineBefore, ",");
			writer.WriteScript(option, WhitespacePadding.NewlineAfter);
			writer.WriteScript(replication, WhitespacePadding.NewlineAfter);
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write("AS");
			writer.IncreaseIndent();
			writer.WriteScript(body, WhitespacePadding.NewlineBefore);
			writer.DecreaseIndent();
		}

		void ICreateOrAlterStatement.WriteToInternal(SqlWriter writer, string command) {
			if (string.IsNullOrEmpty(command)) {
				throw new ArgumentNullException("command");
			}
			WriteToInternal(writer, command);
		}
	}
}
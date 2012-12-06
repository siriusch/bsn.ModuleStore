// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateProcedureStatement: AlterableCreateStatement, ICreateOrAlterStatement {
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
			set {
				procedureName.Name = new ProcedureName(value);
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

		protected override SchemaName SchemaName {
			get {
				return procedureName.Qualification;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteToInternal(writer, "CREATE");
		}

		protected override IInstallStatement CreateAlterStatement() {
			return new AlterOfCreateStatement<CreateProcedureStatement>(this);
		}

		protected override IInstallStatement CreateDropStatement() {
			return new DropProcedureStatement(procedureName);
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

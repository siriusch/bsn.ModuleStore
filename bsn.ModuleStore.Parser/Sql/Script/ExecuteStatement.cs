﻿// bsn ModuleStore database versioning
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
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExecuteStatement: Statement {
		private static readonly Dictionary<string, int[]> objectNameParameters = new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase) {
				{"sp_rename", new[] {0}},
				{"sp_refreshsqlmodule", new[] {0}}
		};

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
			if (!procedureName.IsQualified && procedureName.Name.Value.StartsWith("sp_", StringComparison.OrdinalIgnoreCase)) {
				procedureName.LockOverride();
				if (objectNameParameters.TryGetValue(procedureName.Name.Value, out var parameterPositions)) {
					foreach (var parameterPosition in parameterPositions.Where(p => this.parameters.Count > p)) {
						var objectNameParameter = (ExecuteParameter<Literal>)this.parameters[parameterPosition];
						if ((objectNameParameter != null) && (objectNameParameter.Value is StringLiteral)) {
							this.parameters[parameterPosition] = new ExecuteParameterObjectName(objectNameParameter.ParameterName, ((StringLiteral)objectNameParameter.Value), objectNameParameter.Output);
						}
					}
				}
			}
		}

		[Rule("<ExecuteStatement> ::= ~EXECUTE <ProcedureNameQualified> <ExecuteParameterGroup> <ProcedureOptionGroup>")]
		public ExecuteStatement(Qualified<SchemaName, ProcedureName> procedureName, Optional<Sequence<ExecuteParameter>> parameters, OptionToken option): this(null, procedureName, parameters, option) {}

		public OptionToken Option => option;

		public IEnumerable<ExecuteParameter> Parameters => parameters;

		public Qualified<SchemaName, ProcedureName> ProcedureName => procedureName;

		public VariableName ResultVariableName => resultVariableName;

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteKeyword("EXEC ");
			writer.WriteScript(resultVariableName, WhitespacePadding.None, null, w => w.Write('='));
			writer.WriteScript(procedureName, WhitespacePadding.None);
			writer.WriteScriptSequence(parameters, WhitespacePadding.SpaceBefore, w => w.Write(','));
			writer.WriteScript(option, WhitespacePadding.SpaceBefore);
		}
	}
}

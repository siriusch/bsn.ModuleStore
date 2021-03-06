// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Ars�ne von Wyss - avw@gmx.ch
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

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DeclareTableStatement: DeclareStatement {
		private readonly List<TableDefinition> tableDefinitions;
		private readonly VariableName variableName;

		[Rule("<DeclareStatement> ::= ~DECLARE <VariableName> ~<OptionalAs> ~TABLE <TableDefinitionGroup>")]
		public DeclareTableStatement(VariableName variableName, Sequence<TableDefinition> tableDefinitions) {
			Debug.Assert(variableName != null);
			this.variableName = variableName;
			this.tableDefinitions = tableDefinitions.ToList();
		}

		public IEnumerable<TableDefinition> TableDefinitions => tableDefinitions;

		public VariableName VariableName => variableName;

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteKeyword("DECLARE ");
			writer.WriteScript(variableName, WhitespacePadding.None);
			writer.WriteKeyword(" TABLE ");
			writer.Write('(');
			using (writer.Indent()) {
				writer.WriteScriptSequence(tableDefinitions, WhitespacePadding.NewlineBefore, w => w.Write(','));
			}
			writer.WriteLine();
			writer.Write(')');
		}
	}
}

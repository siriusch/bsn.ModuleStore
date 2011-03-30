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
//  
using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class FetchStatement: Statement {
		private readonly CursorName cursorName;
		private readonly CursorPositionToken cursorPosition;
		private readonly List<VariableName> destinationVariables;

		[Rule("<FetchStatement> ::= ~FETCH <CursorPosition> <GlobalOrLocalCursor>")]
		public FetchStatement(CursorPositionToken cursorPosition, CursorName cursorName): this(cursorPosition, cursorName, null) {}

		[Rule("<FetchStatement> ::= ~FETCH <CursorPosition> <GlobalOrLocalCursor> ~INTO <VariableNameList>")]
		public FetchStatement(CursorPositionToken cursorPosition, CursorName cursorName, Sequence<VariableName> destinationVariables) {
			Debug.Assert(cursorPosition != null);
			Debug.Assert(cursorName != null);
			this.cursorPosition = cursorPosition;
			this.cursorName = cursorName;
			this.destinationVariables = destinationVariables.ToList();
		}

		public CursorName CursorName {
			get {
				return cursorName;
			}
		}

		public CursorPositionToken CursorPosition {
			get {
				return cursorPosition;
			}
		}

		public IEnumerable<VariableName> DestinationVariables {
			get {
				return destinationVariables;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("FETCH ");
			writer.WriteScript(cursorPosition, WhitespacePadding.SpaceAfter);
			writer.WriteScript(cursorName, WhitespacePadding.None);
			if (destinationVariables.Count > 0) {
				writer.IncreaseIndent();
				writer.WriteLine();
				writer.Write("INTO ");
				writer.WriteScriptSequence(destinationVariables, WhitespacePadding.None, ", ");
				writer.DecreaseIndent();
			}
		}
	}
}

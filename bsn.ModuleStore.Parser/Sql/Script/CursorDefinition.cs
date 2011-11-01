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

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CursorDefinition: SqlScriptableToken {
		private readonly List<string> cursorOptions = new List<string>();
		private readonly UpdateMode cursorUpdate;
		private readonly bool global;
		private readonly SelectStatement selectStatement;

		[Rule("<CursorDefinition> ::= ~CURSOR <CursorOptionList> ~FOR <SelectStatement>")]
		public CursorDefinition(Sequence<Identifier> cursorOptions, SelectStatement selectStatement): this(cursorOptions, selectStatement, null) {}

		[Rule("<CursorDefinition> ::= ~CURSOR <CursorOptionList> ~FOR <SelectStatement> <CursorUpdate>")]
		public CursorDefinition(Sequence<Identifier> cursorOptions, SelectStatement selectStatement, UpdateMode cursorUpdate) {
			this.selectStatement = selectStatement;
			this.cursorUpdate = cursorUpdate;
			foreach (Identifier cursorOption in cursorOptions) {
				string option = cursorOption.Value.ToUpperInvariant();
				this.cursorOptions.Add(option);
				if (option.Equals("GLOBAL", StringComparison.Ordinal)) {
					global = true;
				}
			}
		}

		public IEnumerable<string> CursorOptions {
			get {
				return cursorOptions;
			}
		}

		public UpdateMode CursorUpdate {
			get {
				return cursorUpdate;
			}
		}

		public bool Global {
			get {
				return global;
			}
		}

		public SelectStatement SelectStatement {
			get {
				return selectStatement;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CURSOR");
			foreach (string cursorOption in cursorOptions) {
				writer.Write(' ');
				writer.Write(cursorOption);
			}
			writer.Write(" FOR ");
			writer.WriteScript(selectStatement, WhitespacePadding.None);
			writer.WriteScript(cursorUpdate, WhitespacePadding.SpaceBefore);
		}
	}
}

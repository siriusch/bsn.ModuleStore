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
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CursorName: SqlQuotedName {
		private readonly bool global;

		[Rule("<CursorName> ::= Id")]
		[Rule("<CursorName> ::= QuotedId")]
		public CursorName(Identifier identifier): this(identifier.Value, false) {}

		[Rule("<GlobalOrLocalCursor> ::= <VariableName>")]
		public CursorName(VariableName variableName): this(variableName.Value, false) {}

		[Rule("<GlobalOrLocalCursor> ::= Id <CursorName>")]
		public CursorName(Identifier global, CursorName name): this(name.Value, true) {
			if (!string.Equals(global.Value, "GLOBAL", StringComparison.OrdinalIgnoreCase)) {
				throw new ArgumentException("GLOBAl expected", "global");
			}
		}

		private CursorName(string name, bool global): base(name) {
			this.global = global;
		}

		public bool Global {
			get {
				return global;
			}
		}

		protected internal override void WriteToInternal(SqlWriter writer, bool isPartOfQualifiedName) {
			Debug.Assert(!isPartOfQualifiedName);
			if (global) {
				writer.Write("GLOBAL ");
			}
			WriteNonGlobalInternal(writer);
		}

		internal CursorName AsGlobal() {
			if (global) {
				return this;
			}
			return new CursorName(Value, true);
		}

		internal void WriteNonGlobalInternal(SqlWriter writer) {
			base.WriteToInternal(writer, false);
		}
	}
}

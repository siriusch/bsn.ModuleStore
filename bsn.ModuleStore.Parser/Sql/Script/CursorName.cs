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
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CursorName: SqlQuotedName {
		private readonly bool global;
		private readonly bool quote;

		[Rule("<CursorName> ::= Id")]
		[Rule("<CursorName> ::= QuotedId")]
		public CursorName(Identifier identifier): this(identifier.Value, false, true) {}

		[Rule("<GlobalOrLocalCursor> ::= <VariableName>")]
		public CursorName(VariableName variableName): this(variableName.Value, false, false) {}

		[Rule("<GlobalOrLocalCursor> ::= ~GLOBAL <CursorName>")]
		public CursorName(CursorName name): this(name.Value, true, true) {}

		private CursorName(string name, bool global, bool quote): base(name) {
			this.global = global;
			this.quote = quote;
		}

		public bool Global {
			get {
				return global;
			}
		}

		protected internal override void WriteToInternal(SqlWriter writer, bool isPartOfQualifiedName) {
			Debug.Assert(!isPartOfQualifiedName);
			if (global) {
				writer.WriteKeyword("GLOBAL ");
			}
			WriteNonGlobalInternal(writer);
		}

		internal CursorName AsGlobal() {
			if (!quote) {
				throw new InvalidOperationException();
			}
			if (global) {
				return this;
			}
			return new CursorName(Value, true, true);
		}

		internal void WriteNonGlobalInternal(SqlWriter writer) {
			if (!quote) {
				writer.Write(Value);
			} else {
				base.WriteToInternal(writer, false);
			}
		}
	}
}

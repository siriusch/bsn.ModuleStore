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
	public class WaitforStatement: Statement {
		private readonly Identifier identifier;
		private readonly SqlScriptableToken stringValue;

		[Rule("<WaitforStatement> ::= ~WAITFOR Id <StringLiteral>")]
		[Rule("<WaitforStatement> ::= ~WAITFOR Id <VariableName>")]
		public WaitforStatement(Identifier identifier, SqlScriptableToken stringValue) {
			Debug.Assert(identifier != null);
			Debug.Assert(stringValue != null);
			this.identifier = identifier;
			this.stringValue = stringValue;
		}

		public Identifier Identifier {
			get {
				return identifier;
			}
		}

		public SqlScriptableToken StringValue {
			get {
				return stringValue;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("WAIT FOR ");
			writer.WriteScript(identifier, WhitespacePadding.SpaceAfter);
			stringValue.WriteTo(writer);
		}
	}
}

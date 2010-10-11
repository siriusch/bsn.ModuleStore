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

using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class KeywordFunction: FunctionCall {
		private readonly ReservedKeyword keyword;

		[Rule("<FunctionCall> ::= CURRENT_TIMESTAMP")]
		[Rule("<FunctionCall> ::= CURRENT_USER")]
		[Rule("<FunctionCall> ::= SESSION_USER")]
		[Rule("<FunctionCall> ::= SYSTEM_USER")]
		[Rule("<FunctionCall> ::= USER")]
		public KeywordFunction(ReservedKeyword keyword) {
			Debug.Assert(keyword != null);
			this.keyword = keyword;
		}

		public ReservedKeyword Keyword {
			get {
				return keyword;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(keyword, WhitespacePadding.None);
		}
	}
}

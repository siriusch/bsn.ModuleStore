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
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CountFunction: FunctionCall {
		private readonly SqlScriptableToken expression;
		private readonly bool? restriction;

		[Rule("<FunctionCall> ::= ~COUNT ~'(' <Restriction> <ColumnWild> ~')'")]
		[Rule("<FunctionCall> ::= ~COUNT ~'(' <Restriction> <Expression> ~')'")]
		public CountFunction(DuplicateRestrictionToken restriction, SqlScriptableToken expression): this(restriction.Distinct, expression) {}

		[Rule("<FunctionCall> ::= ~COUNT ~'(' <ColumnWild> ~')'")]
		[Rule("<FunctionCall> ::= ~COUNT ~'(' <Expression> ~')'")]
		public CountFunction(SqlScriptableToken expression): this(default(bool?), expression) {}

		private CountFunction(bool? restriction, SqlScriptableToken expression) {
			Debug.Assert(expression != null);
			this.restriction = restriction;
			this.expression = expression;
		}

		public SqlScriptableToken Expression {
			get {
				return expression;
			}
		}

		public bool? Restriction {
			get {
				return restriction;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteFunction("COUNT");
			writer.Write('(');
			writer.WriteDuplicateRestriction(restriction, WhitespacePadding.SpaceAfter);
			writer.WriteScript(expression, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}

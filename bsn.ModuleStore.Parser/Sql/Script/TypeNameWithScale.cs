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
using System.Globalization;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TypeNameWithScale: TypeNameWithPrecision {
		private readonly long scale;

		[Rule("<TypeName> ::= Id ~'(' <IntegerLiteral> ~',' <IntegerLiteral> ~')'")]
		[Rule("<TypeName> ::= QuotedId ~'(' <IntegerLiteral> ~',' <IntegerLiteral> ~')'")]
		public TypeNameWithScale(SqlIdentifier identifier, IntegerLiteral precision, IntegerLiteral scale): base(identifier, precision) {
			Debug.Assert(scale != null);
			this.scale = scale.Value;
		}

		public long Scale {
			get {
				return scale;
			}
		}

		protected override void WriteArguments(SqlWriter writer) {
			base.WriteArguments(writer);
			writer.Write(", ");
			writer.Write(scale.ToString(NumberFormatInfo.InvariantInfo));
		}
	}
}

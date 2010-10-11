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
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CountFunction: FunctionCall {
		private readonly Qualified<SqlName, ColumnName> columnName;
		private readonly bool? restriction;

		[Rule("<FunctionCall> ::= ~COUNT ~'(' <Restriction> <ColumnWildNameQualified> ~')'")]
		public CountFunction(DuplicateRestrictionToken restriction, Qualified<SqlName, ColumnName> columnName): this(restriction.Distinct, columnName) {}

		[Rule("<FunctionCall> ::= ~COUNT ~'(' <ColumnWildNameQualified> ~')'")]
		public CountFunction(Qualified<SqlName, ColumnName> columnName): this(default(bool?), columnName) {}

		private CountFunction(bool? restriction, Qualified<SqlName, ColumnName> columnName) {
			Debug.Assert(columnName != null);
			this.restriction = restriction;
			this.columnName = columnName;
		}

		public Qualified<SqlName, ColumnName> ColumnName {
			get {
				return columnName;
			}
		}

		public bool? Restriction {
			get {
				return restriction;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("COUNT(");
			writer.WriteDuplicateRestriction(restriction, WhitespacePadding.SpaceAfter);
			writer.WriteScript(columnName, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}

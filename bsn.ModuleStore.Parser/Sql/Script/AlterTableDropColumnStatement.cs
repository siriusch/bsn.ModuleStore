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
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class AlterTableDropColumnStatement: AlterTableStatement {
		private readonly ColumnName columnName;

		[Rule("<AlterTableStatement> ::= ~ALTER ~TABLE <TableNameQualified> ~DROP ~COLUMN <ColumnName>")]
		public AlterTableDropColumnStatement(Qualified<SchemaName, TableName> tableName, ColumnName columnName): base(tableName) {
			Debug.Assert(columnName != null);
			this.columnName = columnName;
		}

		public ColumnName ColumnName => columnName;

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteKeyword("DROP COLUMN ");
			writer.WriteScript(columnName, WhitespacePadding.None);
		}
	}
}

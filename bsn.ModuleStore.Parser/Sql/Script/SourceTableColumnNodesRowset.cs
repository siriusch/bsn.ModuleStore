﻿// bsn ModuleStore database versioning
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
	public sealed class SourceTableColumnNodesRowset: SourceNodesRowset {
		private readonly ColumnName columnName;
		private readonly Qualified<SchemaName, TableName> tableNameQualified;

		[Rule("<SourceRowset> ::= <SchemaName> ~'.' <TableName> ~'.' <NamedFunction> <RowsetAlias>")]
		public SourceTableColumnNodesRowset(SchemaName tableName, TableName columnName, NamedFunction functionCall, RowsetAlias rowsetAlias): this(new Qualified<SchemaName, TableName>(null, new TableName(tableName.Value)), new ColumnName(columnName.Value), functionCall, rowsetAlias) {
			tableNameQualified.LockOverride(); // TableName here is usually an alias to a table, so don't prefix this
		}

		//		[Rule("<SourceRowset> ::= <SchemaName> ~'.' <TableName> ~'.' <ColumnName> ~'.' <FunctionCall> <RowsetAlias>")]
		//		public SourceTableColumnNodesRowset(SchemaName schemaName, TableName tableName, ColumnName columnName, ExpressionFunctionCall functionCall, RowsetAlias rowsetAlias): this(new Qualified<SchemaName, TableName>(schemaName, tableName), columnName, functionCall, rowsetAlias) {}

		private SourceTableColumnNodesRowset(Qualified<SchemaName, TableName> tableNameQualified, ColumnName columnName, NamedFunction functionCall, RowsetAlias rowsetAlias): base(functionCall, rowsetAlias) {
			Debug.Assert(tableNameQualified != null);
			Debug.Assert(columnName != null);
			this.tableNameQualified = tableNameQualified;
			this.columnName = columnName;
		}

		public ColumnName ColumnName => columnName;

		public Qualified<SchemaName, TableName> TableNameQualified => tableNameQualified;

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(tableNameQualified, WhitespacePadding.None);
			writer.Write('.');
			writer.WriteScript(columnName, WhitespacePadding.None);
			writer.Write('.');
			base.WriteTo(writer);
		}
	}
}

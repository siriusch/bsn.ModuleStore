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
using System.Collections.Generic;
using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class CreateIndexStatement: AlterableCreateStatement, ITableBound {
		private readonly IndexOptionGroup indexOptions;
		private readonly Qualified<SchemaName, TableName> tableName;
		private IndexName indexName;

		protected CreateIndexStatement(IndexName indexName, Qualified<SchemaName, TableName> tableName, IndexOptionGroup indexOptions) {
			Debug.Assert(indexName != null);
			Debug.Assert(tableName != null);
			this.indexName = indexName;
			this.tableName = tableName;
			this.indexOptions = indexOptions;
		}

		public IndexName IndexName => indexName;

		public IndexOptionGroup IndexOptions => indexOptions;

		public override sealed ObjectCategory ObjectCategory => ObjectCategory.Index;

		public override string ObjectName {
			get => indexName.Value;
			set => indexName = new IndexName(value);
		}

		public Qualified<SchemaName, TableName> TableName => tableName;

		protected override SchemaName SchemaName => tableName.Qualification;

		protected override IInstallStatement CreateDropStatement() {
			return new DropIndexStatement(indexName, tableName, null);
		}
	}
}

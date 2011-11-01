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
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TableForeignKeyConstraint: TableConstraint {
		private readonly List<ColumnName> columnNames;
		private readonly List<ForeignKeyAction> keyActions;
		private readonly List<ColumnName> refColumnNames;
		private readonly Qualified<SchemaName, TableName> refTableName;

		[Rule("<TableConstraint> ::= ~FOREIGN ~KEY ~'(' <ColumnNameList> ~')' ~REFERENCES <TableNameQualified> <ColumnNameGroup> <ForeignKeyActionList>")]
		public TableForeignKeyConstraint(Sequence<ColumnName> columnNames, Qualified<SchemaName, TableName> refTableName, Optional<Sequence<ColumnName>> refColumnNames, Sequence<ForeignKeyAction> keyActions): this(null, columnNames, refTableName, refColumnNames, keyActions) {}

		[Rule("<TableConstraint> ::= ~CONSTRAINT <ConstraintName> ~FOREIGN ~KEY ~'(' <ColumnNameList> ~')' ~REFERENCES <TableNameQualified> <ColumnNameGroup> <ForeignKeyActionList>")]
		public TableForeignKeyConstraint(ConstraintName constraintName, Sequence<ColumnName> columnNames, Qualified<SchemaName, TableName> refTableName, Optional<Sequence<ColumnName>> refColumnNames, Sequence<ForeignKeyAction> keyActions): base(constraintName) {
			Debug.Assert(refTableName != null);
			this.columnNames = columnNames.ToList();
			this.refTableName = refTableName;
			this.refColumnNames = refColumnNames.ToList();
			this.keyActions = keyActions.ToList();
		}

		public IEnumerable<ColumnName> ColumnNames {
			get {
				return columnNames;
			}
		}

		public IEnumerable<ForeignKeyAction> KeyActions {
			get {
				return keyActions;
			}
		}

		public IEnumerable<ColumnName> RefColumnNames {
			get {
				return refColumnNames;
			}
		}

		public Qualified<SchemaName, TableName> RefTableName {
			get {
				return refTableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("FOREIGN KEY (");
			writer.WriteScriptSequence(columnNames, WhitespacePadding.None, ", ");
			writer.Write(") REFERENCES ");
			writer.WriteScript(refTableName, WhitespacePadding.None);
			if (refColumnNames.Count > 0) {
				writer.Write(" (");
				writer.WriteScriptSequence(refColumnNames, WhitespacePadding.None, ", ");
				writer.Write(')');
			}
			writer.WriteScriptSequence(keyActions, WhitespacePadding.SpaceBefore, null);
		}
	}
}

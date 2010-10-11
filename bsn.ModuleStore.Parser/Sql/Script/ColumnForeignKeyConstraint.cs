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
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnForeignKeyConstraint: ColumnNamedConstraintBase {
		private readonly List<ForeignKeyAction> keyActions;
		private readonly ColumnName refColumnName;
		private readonly Qualified<SchemaName, TableName> tableName;

		[Rule("<NamedColumnConstraint> ::= ~REFERENCES <TableNameQualified> <OptionalForeignRefColumn> <ForeignKeyActionList>")]
		[Rule("<NamedColumnConstraint> ::= ~FOREIGN ~KEY ~REFERENCES <TableNameQualified> <OptionalForeignRefColumn> <ForeignKeyActionList>")]
		public ColumnForeignKeyConstraint(Qualified<SchemaName, TableName> tableName, Optional<ColumnName> refColumnName, Sequence<ForeignKeyAction> keyActions): this(null, tableName, refColumnName, keyActions) {}

		[Rule("<NamedColumnConstraint> ::= ~CONSTRAINT <ConstraintName> ~REFERENCES <TableNameQualified> <OptionalForeignRefColumn> <ForeignKeyActionList>")]
		[Rule("<NamedColumnConstraint> ::= ~CONSTRAINT <ConstraintName> ~FOREIGN ~KEY ~REFERENCES <TableNameQualified> <OptionalForeignRefColumn> <ForeignKeyActionList>")]
		public ColumnForeignKeyConstraint(ConstraintName constraintName, Qualified<SchemaName, TableName> tableName, Optional<ColumnName> refColumnName, Sequence<ForeignKeyAction> keyActions): base(constraintName) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
			this.refColumnName = refColumnName;
			this.keyActions = keyActions.ToList();
		}

		public IEnumerable<ForeignKeyAction> KeyActions {
			get {
				return keyActions;
			}
		}

		public ColumnName RefColumnName {
			get {
				return refColumnName;
			}
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("REFERENCES ");
			writer.WriteScript(tableName, WhitespacePadding.None);
			writer.WriteScript(refColumnName, WhitespacePadding.SpaceBefore);
			writer.WriteScriptSequence(keyActions, WhitespacePadding.SpaceBefore, null);
		}
	}
}

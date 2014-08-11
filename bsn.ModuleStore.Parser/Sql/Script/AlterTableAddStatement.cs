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
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class AlterTableAddStatement: AlterTableStatement, IApplicableTo<CreateTableStatement>, IObjectBoundStatement {
		private readonly TableCheck check;
		private readonly List<TableDefinition> definitions;

		[Rule("<AlterTableStatement> ::= ~ALTER ~TABLE <TableNameQualified> <TableCheck> ~ADD <TableDefinitionList>")]
		public AlterTableAddStatement(Qualified<SchemaName, TableName> tableName, TableCheckToken check, Sequence<TableDefinition> definitions): base(tableName) {
			Debug.Assert(check != null);
			Debug.Assert(definitions != null);
			this.check = check.TableCheck;
			this.definitions = definitions.ToList();
		}

		public IEnumerable<TableDefinition> Definitions {
			get {
				return definitions;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteEnum(check, WhitespacePadding.SpaceAfter);
			writer.WriteKeyword("ADD ");
			writer.WriteScriptSequence(definitions, WhitespacePadding.None, w => w.Write(", "));
		}

		IQualifiedName<SchemaName> IApplicableTo<CreateTableStatement>.QualifiedName {
			get {
				return TableName;
			}
		}

		public void ApplyTo(CreateTableStatement instance) {
			instance.Definitions.AddRange(definitions);
		}

		string IObjectBoundStatement.ObjectName {
			get {
				return TableName.Name.Value;
			}
		}
	}
}

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
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script;

[assembly: RuleTrim("<TableDefinitionGroup> ::= '(' <TableDefinitionList> ')'", "<TableDefinitionList>", SemanticTokenType = typeof(SqlToken))]

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateTableStatement: CreateStatement {
		private readonly List<TableDefinition> definitions;
		private readonly Qualified<SchemaName, TableName> tableName;

		[Rule("<CreateTableStatement> ::= ~CREATE ~TABLE <TableNameQualified> <TableDefinitionGroup>")]
		public CreateTableStatement(Qualified<SchemaName, TableName> tableName, Sequence<TableDefinition> definitions) {
			this.tableName = tableName;
			this.definitions = definitions.ToList();
		}

		public List<TableDefinition> Definitions {
			get {
				return definitions;
			}
		}

		public override ObjectCategory ObjectCategory {
			get {
				return ObjectCategory.Table;
			}
		}

		public override string ObjectName {
			get {
				return tableName.Name.Value;
			}
			set {
				tableName.Name = new TableName(value);
			}
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}

		public override Statement CreateAlterStatement() {
			throw new NotSupportedException("Tables must be altered via change scripts");
		}

		public override DropStatement CreateDropStatement() {
			return new DropTableStatement(tableName);
		}

		public override void WriteTo(SqlWriter writer) {
			WriteTo(writer, definition => definition);
		}

		public void WriteTo(SqlWriter writer, Func<TableDefinition, TableDefinition> definitionRewriter) {
			if (definitionRewriter == null) {
				throw new ArgumentNullException("definitionRewriter");
			}
			WriteCommentsTo(writer);
			writer.Write("CREATE TABLE ");
			writer.WriteScript(tableName, WhitespacePadding.None);
			writer.Write(" (");
			writer.IncreaseIndent();
			writer.WriteScriptSequence(definitions.Select(definitionRewriter), WhitespacePadding.NewlineBefore, ",");
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(')');
		}

		protected override string GetObjectSchema() {
			return tableName.IsQualified ? tableName.Qualification.Value : string.Empty;
		}
	}
}
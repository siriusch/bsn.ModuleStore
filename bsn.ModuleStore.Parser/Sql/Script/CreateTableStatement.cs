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

		public List<TableDefinition> Definitions => definitions;

		public override ObjectCategory ObjectCategory => ObjectCategory.Table;

		public override string ObjectName {
			get => tableName.Name.Value;
			set => tableName.Name = new TableName(value);
		}

		public Qualified<SchemaName, TableName> TableName => tableName;

		protected override SchemaName SchemaName => tableName.Qualification;

		public override IEnumerable<IAlterableCreateStatement> CreateStatementFragments(CreateFragmentMode mode) {
			var definitions = new List<TableDefinition>();
			var constraints = new List<TableConstraint>();
			foreach (var definition in Definitions) {
				if ((!(definition is TableConstraint constraint)) || ((mode == CreateFragmentMode.CreateOnExistingSchema) && (constraint is TableUniqueConstraintBase)) || ((mode == CreateFragmentMode.CreateOnNewSchema) && constraint.IsPartOfSchemaDefinition)) {
					definitions.Add(definition);
				} else {
					constraints.Add(constraint);
				}
			}
			yield return new CreateTableFragment(this, definitions);
			foreach (var constraint in constraints) {
				yield return new AlterTableAddConstraintFragment(this, constraint);
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteTo(writer, definition => definition);
		}

		internal void WriteTo(SqlWriter writer, Func<TableDefinition, TableDefinition> definitionRewriter) {
			if (definitionRewriter == null) {
				throw new ArgumentNullException(nameof(definitionRewriter));
			}
			WriteCommentsTo(writer);
			writer.WriteKeyword("CREATE TABLE ");
			writer.WriteScript(tableName, WhitespacePadding.None);
			writer.Write(" (");
			using (writer.Indent()) {
				var tableDefinitions = definitions.Select(definitionRewriter);
				if (writer.Mode == SqlWriterMode.ForHashing) {
					tableDefinitions = tableDefinitions.OrderBy(td => (td != null) ? td.ToString(DatabaseEngine.Unknown) : string.Empty);
				}
				writer.WriteScriptSequence(tableDefinitions, WhitespacePadding.NewlineBefore, w => w.Write(','));
			}
			writer.WriteLine();
			writer.Write(')');
		}
	}
}

using System;
using System.Collections.Generic;

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
			WriteCommentsTo(writer);
			writer.Write("CREATE TABLE ");
			writer.WriteScript(tableName, WhitespacePadding.None);
			writer.Write(" (");
			writer.IncreaseIndent();
			writer.WriteScriptSequence(definitions, WhitespacePadding.NewlineBefore, ",");
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(")");
		}

		protected override string GetObjectSchema() {
			return tableName.IsQualified ? tableName.Qualification.Value : string.Empty;
		}
	}
}

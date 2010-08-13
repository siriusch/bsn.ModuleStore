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

		[Rule("<CreateTableStatement> ::= CREATE TABLE <TableNameQualified> <TableDefinitionGroup>", ConstructorParameterMapping = new[] {2, 3})]
		public CreateTableStatement(Qualified<SchemaName, TableName> tableName, Sequence<TableDefinition> definitions) {
			this.tableName = tableName;
			this.definitions = definitions.ToList();
		}

		public List<TableDefinition> Definitions {
			get {
				return definitions;
			}
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CREATE TABLE ");
			writer.WriteScript(tableName, WhitespacePadding.None);
			writer.Write(" (");
			writer.IncreaseIndent();
			writer.WriteSequence(definitions, WhitespacePadding.NewlineBefore, ",");
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(")");
		}
	}
}
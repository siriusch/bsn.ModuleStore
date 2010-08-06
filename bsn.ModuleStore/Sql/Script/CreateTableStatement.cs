﻿using System;
using System.Collections.Generic;
using System.Linq;

using bsn.GoldParser.Semantic;

[assembly: RuleTrim("<TableDefinitionGroup> ::= '(' <TableDefinitionList> ')'", "<TableDefinitionList>")]

namespace bsn.ModuleStore.Sql.Script {
	public class CreateTableStatement: SqlCreateStatement {
		private readonly List<TableDefinition> definitions;
		private readonly TableName tableName;

		[Rule("<CreateTableStatement> ::= CREATE TABLE <TableName> <TableDefinitionGroup>", ConstructorParameterMapping = new[] {2, 3})]
		public CreateTableStatement(TableName tableName, Sequence<TableDefinition> definitions) {
			this.tableName = tableName;
			this.definitions = definitions.ToList();
		}

		public List<TableDefinition> Definitions {
			get {
				return definitions;
			}
		}

		public TableName TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("CREATE TABLE ");
			tableName.WriteTo(writer);
			writer.WriteLine(" (");
			writer.WriteSequence(definitions, "\t", ",", Environment.NewLine);
			writer.WriteLine(")");
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class AlterTableAddStatement: AlterTableStatement {
		private readonly TableCheck check;
		private readonly List<TableDefinition> definitions;

		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableName> <TableCheck> ADD <TableDefinitionList>", ConstructorParameterMapping = new[] {2, 3, 5})]
		public AlterTableAddStatement(TableName tableName, TableCheckToken check, Sequence<TableDefinition> definitions): base(tableName) {
			if (check == null) {
				throw new ArgumentNullException("check");
			}
			if (definitions == null) {
				throw new ArgumentNullException("definitions");
			}
			this.check = check.TableCheck;
			this.definitions = definitions.ToList();
		}

		public override void ApplyTo(CreateTableStatement createTable) {
			throw new NotImplementedException();
		}

		public override void WriteTo(TextWriter writer) {
			throw new NotImplementedException();
		}
	}
}
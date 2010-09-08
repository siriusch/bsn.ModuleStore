using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class AlterTableAddStatement: AlterTableStatement {
		private readonly TableCheck check;
		private readonly List<TableDefinition> definitions;

		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableNameQualified> <TableCheck> ADD <TableDefinitionList>", ConstructorParameterMapping = new[] {2, 3, 5})]
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
			writer.Write("ADD ");
			writer.WriteScriptSequence(definitions, WhitespacePadding.None, ", ");
		}
	}
}
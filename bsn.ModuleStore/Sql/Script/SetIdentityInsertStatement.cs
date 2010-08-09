using System;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SetIdentityInsertStatement: SqlStatement {
		private readonly bool enabled;
		private readonly TableName tableName;

		[Rule("<SetOptionStatement> ::= SET IDENTITY_INSERT <TableName> <Toggle>", ConstructorParameterMapping = new[] {2, 3})]
		public SetIdentityInsertStatement(TableName tableNameName, ToggleToken toggle) {
			if (tableNameName == null) {
				throw new ArgumentNullException("tableNameName");
			}
			if (toggle == null) {
				throw new ArgumentNullException("toggle");
			}
			this.tableName = tableNameName;
			enabled = toggle.On;
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("SET IDENTITY INSERT ");
			writer.WriteScript(tableName);
			writer.WriteToggle(enabled, " ", null);
		}
	}
}
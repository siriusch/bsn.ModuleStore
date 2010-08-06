using System;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class SetIdentityInsertStatement: SqlStatement {
		private readonly bool enabled;
		private readonly TableName table;

		[Rule("<SetOptionStatement> ::= SET IDENTITY_INSERT <TableName> <Toggle>", ConstructorParameterMapping = new[] {2, 3})]
		public SetIdentityInsertStatement(TableName tableName, ToggleToken toggle) {
			if (tableName == null) {
				throw new ArgumentNullException("tableName");
			}
			if (toggle == null) {
				throw new ArgumentNullException("toggle");
			}
			table = tableName;
			enabled = toggle.On;
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("SET IDENTITY INSERT ");
			table.WriteTo(writer);
			writer.Write(enabled ? " ON" : " OFF");
		}
	}
}
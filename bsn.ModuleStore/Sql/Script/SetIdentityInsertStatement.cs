using System;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SetIdentityInsertStatement: Statement {
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
			tableName = tableNameName;
			enabled = toggle.On;
		}

		public bool Enabled {
			get {
				return enabled;
			}
		}

		public TableName TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("SET IDENTITY INSERT ");
			writer.WriteScript(tableName);
			writer.WriteToggle(enabled, " ", null);
		}
	}
}
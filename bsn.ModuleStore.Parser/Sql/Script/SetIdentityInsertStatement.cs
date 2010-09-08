using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SetIdentityInsertStatement: Statement {
		private readonly bool enabled;
		private readonly TableName tableName;

		[Rule("<SetOptionStatement> ::= SET IDENTITY_INSERT <TableName> <Toggle>", ConstructorParameterMapping = new[] {2, 3})]
		public SetIdentityInsertStatement(TableName tableName, ToggleToken toggle) {
			Debug.Assert(tableName != null);
			Debug.Assert(toggle != null);
			this.tableName = tableName;
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

		public override void WriteTo(SqlWriter writer) {
			writer.Write("SET IDENTITY INSERT ");
			writer.WriteScript(tableName, WhitespacePadding.None);
			writer.WriteToggle(enabled, WhitespacePadding.SpaceBefore);
		}
	}
}
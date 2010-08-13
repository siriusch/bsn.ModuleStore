using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TriggerTableTarget: TriggerTarget {
		private readonly Qualified<SchemaName, TableName> tableName;

		[Rule("<TriggerTarget> ::= <TableNameQualified>")]
		public TriggerTableTarget(Qualified<SchemaName, TableName> tableName) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
		}

		public Qualified<SchemaName, TableName> TableName1 {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(tableName, WhitespacePadding.None);
		}
	}
}
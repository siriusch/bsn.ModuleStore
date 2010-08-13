using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class OpenxmlImplicitSchema: OpenxmlSchema {
		private readonly Qualified<SchemaName, TableName> tableName;

		[Rule("<OpenxmlImplicitSchema> ::= WITH '(' <TableNameQualified> ')'", ConstructorParameterMapping = new[] {2})]
		public OpenxmlImplicitSchema(Qualified<SchemaName, TableName> tableName) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}

		protected override void WriteToInternal(SqlWriter writer) {
			writer.WriteScript(tableName, WhitespacePadding.None);
		}
	}
}
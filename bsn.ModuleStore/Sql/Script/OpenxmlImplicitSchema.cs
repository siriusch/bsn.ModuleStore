using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class OpenxmlImplicitSchema: OpenxmlSchema {
		private readonly Qualified<TableName> tableName;

		[Rule("<OpenxmlImplicitSchema> ::= WITH '(' <TableNameQualified> ')'", ConstructorParameterMapping = new[] {2})]
		public OpenxmlImplicitSchema(Qualified<TableName> tableName) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
		}

		public Qualified<TableName> TableName {
			get {
				return tableName;
			}
		}

		protected override void WriteToInternal(TextWriter writer) {
			writer.WriteScript(tableName);
		}
	}
}
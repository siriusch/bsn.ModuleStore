using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceTableRowset: SourceRowset {
		private readonly Qualified<TableName> tableName;

		[Rule("<SourceRowset> ::= <TableNameQualified> <OptionalAlias>")]
		public SourceTableRowset(Qualified<TableName> tableName, Optional<AliasName> aliasName): base(aliasName) {
			if (tableName == null) {
				throw new ArgumentNullException("tableName");
			}
			this.tableName = tableName;
		}

		public Qualified<TableName> TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteScript(tableName);
			base.WriteTo(writer);
		}
	}
}
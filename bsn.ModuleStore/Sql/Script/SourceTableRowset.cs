﻿using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceTableRowset: SourceRowset {
		private readonly Qualified<TableName> tableName;

		[Rule("<SourceRowset> ::= <TableNameQualified> <OptionalAlias>")]
		public SourceTableRowset(Qualified<TableName> tableName, Optional<AliasName> aliasName): base(aliasName) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
		}

		public Qualified<TableName> TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(tableName, WhitespacePadding.None);
			base.WriteTo(writer);
		}
	}
}
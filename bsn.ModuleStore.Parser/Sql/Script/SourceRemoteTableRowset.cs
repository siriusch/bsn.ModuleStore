using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceRemoteTableRowset: SourceRowset {
		private readonly Identifier databaseName;
		private readonly SchemaName schemaName;
		private readonly Identifier serverName;
		private readonly TableName tableName;

		[Rule("<SourceRowset> ::= <SchemaName> ~'.' <TableName> ~'.' <TableName> <RowsetAlias>")]
		public SourceRemoteTableRowset(SchemaName databaseName, TableName schemaName, TableName tableName, RowsetAlias rowsetAlias): this(string.Empty, databaseName.Value, new SchemaName(schemaName.Value), tableName, rowsetAlias) {}

		[Rule("<SourceRowset> ::= <SchemaName> ~'.' <TableName> ~'.' <SchemaName> ~'.' <TableName> <RowsetAlias>")]
		public SourceRemoteTableRowset(SchemaName serverName, TableName databaseName, SchemaName schemaName, TableName tableName, RowsetAlias rowsetAlias): this(serverName.Value, databaseName.Value, schemaName, tableName, rowsetAlias) {}

		private SourceRemoteTableRowset(string serverName, string databaseName, SchemaName schemaName, TableName tableName, RowsetAlias rowsetAlias): base(rowsetAlias) {
			Debug.Assert(tableName != null);
			if (!string.IsNullOrEmpty(serverName)) {
				this.serverName = new Identifier(serverName);
			}
			this.databaseName = new Identifier(databaseName);
			this.schemaName = schemaName;
			this.tableName = tableName;
		}

		public Identifier DatabaseName {
			get {
				return databaseName;
			}
		}

		public SchemaName SchemaName {
			get {
				return schemaName;
			}
		}

		public Identifier ServerName {
			get {
				return serverName;
			}
		}

		public TableName TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(serverName, WhitespacePadding.None, null, ".");
			writer.WriteScript(databaseName, WhitespacePadding.None, null, ".");
			writer.WriteScript(schemaName, WhitespacePadding.None, null, ".");
			writer.WriteScript(tableName, WhitespacePadding.None);
			base.WriteTo(writer);
		}
	}
}
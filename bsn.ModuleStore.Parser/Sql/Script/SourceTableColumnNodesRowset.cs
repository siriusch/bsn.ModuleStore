using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceTableColumnNodesRowset: SourceNodesRowset {
		private readonly ColumnName columnName;
		private readonly Qualified<SchemaName, TableName> tableNameQualified;

		[Rule("<SourceRowset> ::= <SchemaName> ~'.' <TableName> ~'.' <NamedFunction> <RowsetAlias>")]
		public SourceTableColumnNodesRowset(SchemaName tableName, TableName columnName, NamedFunction functionCall, RowsetAlias rowsetAlias): this(new Qualified<SchemaName, TableName>(null, new TableName(tableName.Value)), new ColumnName(columnName.Value), functionCall, rowsetAlias) {}

//		[Rule("<SourceRowset> ::= <SchemaName> ~'.' <TableName> ~'.' <ColumnName> ~'.' <FunctionCall> <RowsetAlias>")]
//		public SourceTableColumnNodesRowset(SchemaName schemaName, TableName tableName, ColumnName columnName, ExpressionFunctionCall functionCall, RowsetAlias rowsetAlias): this(new Qualified<SchemaName, TableName>(schemaName, tableName), columnName, functionCall, rowsetAlias) {}

		private SourceTableColumnNodesRowset(Qualified<SchemaName, TableName> tableNameQualified, ColumnName columnName, NamedFunction functionCall, RowsetAlias rowsetAlias): base(functionCall, rowsetAlias) {
			Debug.Assert(tableNameQualified != null);
			Debug.Assert(columnName != null);
			this.tableNameQualified = tableNameQualified;
			this.columnName = columnName;
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public Qualified<SchemaName, TableName> TableNameQualified {
			get {
				return tableNameQualified;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(tableNameQualified, WhitespacePadding.None);
			writer.Write('.');
			writer.WriteScript(columnName, WhitespacePadding.None);
			writer.Write('.');
			base.WriteTo(writer);
		}
	}
}
using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SourceNodesRowset: SourceRowset {
		private readonly StringLiteral xQuery;
		private readonly ColumnName virtualColumn;

		protected SourceNodesRowset(StringLiteral xQuery, AliasName virtualTable, ColumnName virtualColumn): base(virtualTable) {
			Debug.Assert(xQuery != null);
			Debug.Assert(virtualTable != null);
			this.xQuery = xQuery;
			this.virtualColumn = virtualColumn;
		}

		public StringLiteral XQuery {
			get {
				return xQuery;
			}
		}

		public ColumnName VirtualColumn {
			get {
				return virtualColumn;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write(".nodes(");
			writer.WriteScript(xQuery, WhitespacePadding.None);
			writer.Write(')');
			base.WriteTo(writer);
			writer.Write('(');
			writer.WriteScript(virtualColumn, WhitespacePadding.None);
			writer.Write(')');
		}
	}

	public sealed class SourceVariableNodesRowset: SourceNodesRowset {
		private readonly VariableName variableName;

		[Rule("<SourceRowset> ::= <VariableName> ~_NODES_ <StringLiteral> ~')' ~<OptionalAs> <AliasName> ~'(' <ColumnName> ~')'")]
		public SourceVariableNodesRowset(VariableName variableName, StringLiteral xQuery, AliasName virtualTable, ColumnName virtualColumn): base(xQuery, virtualTable, virtualColumn) {
			this.variableName = variableName;
		}

		public VariableName VariableName {
			get {
				return variableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(variableName, WhitespacePadding.None);
			base.WriteTo(writer);
		}
	}

	public sealed class SourceTableVariableNodesRowset: SourceNodesRowset {
		private readonly VariableName tableName;
		private readonly ColumnName columnName;

		[Rule("<SourceRowset> ::= <VariableName> ~'.' <ColumnName> ~_NODES_ <StringLiteral> ~')' ~<OptionalAs> <AliasName> ~'(' <ColumnName> ~')'")]
		public SourceTableVariableNodesRowset(VariableName tableName, ColumnName columnName, StringLiteral xQuery, AliasName virtualTable, ColumnName virtualColumn)
			: base(xQuery, virtualTable, virtualColumn) {
			Debug.Assert(tableName != null);
			Debug.Assert(columnName != null);
			this.tableName = tableName;
			this.columnName = columnName;
		}

		public VariableName TableName {
			get {
				return tableName;
			}
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(tableName, WhitespacePadding.None);
			writer.Write('.');
			writer.WriteScript(columnName, WhitespacePadding.None);
			base.WriteTo(writer);
		}
	}

	public sealed class SourceTableColumnNodesRowset: SourceNodesRowset {
		private readonly Qualified<SchemaName, TableName> tableNameQualified;
		private readonly ColumnName columnName;

		[Rule("<SourceRowset> ::= <SchemaName> ~'.' <TableName> ~_NODES_ <StringLiteral> ~')' ~<OptionalAs> <AliasName> ~'(' <ColumnName> ~')'")]
		public SourceTableColumnNodesRowset(SchemaName tableName, TableName columnName, StringLiteral xQuery, AliasName virtualTable, ColumnName virtualColumn)
			: this(new Qualified<SchemaName, TableName>(null, new TableName(tableName.Value)), new ColumnName(columnName.Value), xQuery, virtualTable, virtualColumn) {
		}

		[Rule("<SourceRowset> ::= <SchemaName> ~'.' <TableName> ~'.' <ColumnName> ~_NODES_ <StringLiteral> ~')' ~<OptionalAs> <AliasName> ~'(' <ColumnName> ~')'")]
		public SourceTableColumnNodesRowset(SchemaName schemaName, TableName tableName, ColumnName columnName, StringLiteral xQuery, AliasName virtualTable, ColumnName virtualColumn)
			: this(new Qualified<SchemaName, TableName>(schemaName, tableName), columnName, xQuery, virtualTable, virtualColumn) {
		}

		private SourceTableColumnNodesRowset(Qualified<SchemaName, TableName> tableNameQualified, ColumnName columnName, StringLiteral xQuery, AliasName virtualTable, ColumnName virtualColumn): base(xQuery, virtualTable, virtualColumn) {
			Debug.Assert(tableNameQualified != null);
			Debug.Assert(columnName != null);
			this.tableNameQualified = tableNameQualified;
			this.columnName = columnName;
		}

		public Qualified<SchemaName, TableName> TableNameQualified {
			get {
				return tableNameQualified;
			}
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(tableNameQualified, WhitespacePadding.None);
			writer.Write('.');
			writer.WriteScript(columnName, WhitespacePadding.None);
			base.WriteTo(writer);
		}
	}
}
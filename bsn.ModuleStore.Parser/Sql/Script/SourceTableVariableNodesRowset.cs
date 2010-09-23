using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceTableVariableNodesRowset: SourceNodesRowset {
		private readonly ColumnName columnName;
		private readonly VariableName tableName;

		[Rule("<SourceRowset> ::= <VariableName> ~'.' <ColumnName> ~'.' <NamedFunction> <RowsetAlias>")]
		public SourceTableVariableNodesRowset(VariableName tableName, ColumnName columnName, NamedFunction functionCall, RowsetAlias rowsetAlias): base(functionCall, rowsetAlias) {
			Debug.Assert(tableName != null);
			Debug.Assert(columnName != null);
			this.tableName = tableName;
			this.columnName = columnName;
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public VariableName TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(tableName, WhitespacePadding.None);
			writer.Write('.');
			writer.WriteScript(columnName, WhitespacePadding.None);
			base.WriteTo(writer);
		}
	}
}
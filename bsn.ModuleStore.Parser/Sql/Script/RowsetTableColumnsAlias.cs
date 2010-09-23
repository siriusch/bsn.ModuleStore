using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class RowsetTableColumnsAlias: RowsetTableAlias {
		private readonly List<ColumnName> columnNames;

		[Rule("<RowsetAlias> ::= ~<OptionalAs> <AliasName> ~'(' <ColumnNameList> ~')'")]
		public RowsetTableColumnsAlias(AliasName aliasName, Sequence<ColumnName> columnNames): base(aliasName) {
			Debug.Assert(columnNames != null);
			this.columnNames = columnNames.ToList();
		}

		public IEnumerable<ColumnName> ColumnNames {
			get {
				return columnNames;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write('(');
			writer.WriteScriptSequence(columnNames, WhitespacePadding.None, ", ");
			writer.Write(')');
		}
	}
}
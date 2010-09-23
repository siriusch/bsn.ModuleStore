using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceTableVariableRowset: SourceRowset {
		private readonly VariableName tableName;

		[Rule("<SourceRowset> ::= <VariableName> <RowsetAlias>")]
		public SourceTableVariableRowset(VariableName tableName, RowsetAlias rowsetAlias): base(rowsetAlias) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
		}

		public VariableName TableName {
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
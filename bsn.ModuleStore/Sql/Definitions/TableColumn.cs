// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Runtime.Serialization;

namespace bsn.ModuleStore.Sql.Definitions {
	[Serializable]
	public class TableColumn: SqlColumn<TableColumn> {
		private readonly ColumnFlags columnFlags;

		public TableColumn(string name, SqlType type, ColumnFlags flags): base(name, type) {
			columnFlags = flags;
		}

		protected TableColumn(SerializationInfo info, StreamingContext context): base(info, context) {
			columnFlags = (ColumnFlags)info.GetValue("columnFlags", typeof(ColumnFlags));
		}

		protected override void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("columnFlags", columnFlags);
		}

		protected override bool EqualsInternal(TableColumn other) {
			return base.EqualsInternal(other) && (columnFlags == other.columnFlags);
		}

		public override int GetHashCode() {
			return base.GetHashCode()^(int)columnFlags;
		}
	}
}
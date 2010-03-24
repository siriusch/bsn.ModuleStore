// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Runtime.Serialization;

namespace bsn.ModuleStore.Sql.Definitions {
	[Serializable]
	public class TableColumn: SqlColumn<TableColumn> {
		private readonly ColumnFlags columnFlags;
		private readonly string @default;

		public TableColumn(string name, SqlType type, ColumnFlags flags, string @default): base(name, type) {
			columnFlags = flags;
			this.@default = @default;
		}

		protected TableColumn(SerializationInfo info, StreamingContext context): base(info, context) {
			columnFlags = (ColumnFlags)info.GetValue("columnFlags", typeof(ColumnFlags));
			@default = info.GetString("default");
		}

		protected override void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("columnFlags", columnFlags);
			info.AddValue("default", @default);
		}

		protected override bool EqualsInternal(TableColumn other) {
			return base.EqualsInternal(other) && (columnFlags == other.columnFlags) && (@default == other.@default);
		}

		public override int GetHashCode() {
			int result = base.GetHashCode()^(int)columnFlags;
			if (@default != null) {
				result ^= @default.GetHashCode();
			}
			return result;
		}

		public bool HasFlag(ColumnFlags flag) {
			return (columnFlags & flag)==flag;
		}
		
		public bool NotNull {
			get {
				return HasFlag(ColumnFlags.NotNull);
			}
		}

		public bool Identity {
			get {
				return HasFlag(ColumnFlags.Indentity);
			}
		}

		public bool RowGuid {
			get {
				return HasFlag(ColumnFlags.RowGuid);
			}
		}

		public bool PrimaryKey {
			get {
				return HasFlag(ColumnFlags.PrimaryKey);
			}
		}

		public string Default {
			get {
				return @default;
			}
		}
	}
}
// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Runtime.Serialization;

namespace bsn.ModuleStore.Sql.Definitions {
	public abstract class SqlColumn<TSelf>: Metadata<TSelf> where TSelf: SqlColumn<TSelf> {
		private readonly SqlType sqlType;

		protected SqlColumn(string name, SqlType type): base(name) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			sqlType = type;
		}

		protected SqlColumn(SerializationInfo info, StreamingContext context): base(info, context) {
			sqlType = SqlType.UnifyType((SqlType)info.GetValue("sqlType", typeof(SqlType)));
		}

		protected override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("sqlType", sqlType);
		}

		protected override bool EqualsInternal(TSelf other) {
			return base.EqualsInternal(other) && sqlType.Equals(other.sqlType);
		}

		public override int GetHashCode() {
			return base.GetHashCode()^sqlType.GetHashCode();
		}
	}
}
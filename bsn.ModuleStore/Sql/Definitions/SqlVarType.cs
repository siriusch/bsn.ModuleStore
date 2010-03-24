// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Runtime.Serialization;

namespace bsn.ModuleStore.Sql.Definitions {
	[Serializable]
	public class SqlVarType: SqlType {
		private readonly int length;

		protected internal SqlVarType(string name, int? length): base(name) {
			this.length = length.GetValueOrDefault();
		}

		protected SqlVarType(SerializationInfo info, StreamingContext context): base(info, context) {
			length = info.GetInt32("length");
		}

		public int Length {
			get {
				return length;
			}
		}

		protected override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("length", length);
		}

		protected override bool EqualsInternal(SqlType other) {
			if (base.EqualsInternal(other)) {
				SqlVarType varOther = other as SqlVarType;
				if (varOther != null) {
					return varOther.length == length;
				}
			}
			return false;
		}

		public override int GetHashCode() {
			return base.GetHashCode()^length;
		}
	}
}
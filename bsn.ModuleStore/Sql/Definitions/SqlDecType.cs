// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Runtime.Serialization;

namespace bsn.ModuleStore.Sql.Definitions {
	[Serializable]
	public class SqlDecType: SqlType {
		private readonly int precision;
		private readonly int scale;

		protected internal SqlDecType(string name, int? precision, int? scale): base(name) {
			this.precision = precision.GetValueOrDefault(38);
			this.scale = scale.GetValueOrDefault(0);
		}

		protected SqlDecType(SerializationInfo info, StreamingContext context): base(info, context) {
			precision = info.GetInt32("precision");
			scale = info.GetInt32("scale");
		}

		public int Precision {
			get {
				return precision;
			}
		}

		protected override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("precision", precision);
			info.AddValue("scale", scale);
		}

		protected override bool EqualsInternal(SqlType other) {
			if (base.EqualsInternal(other)) {
				SqlDecType varOther = other as SqlDecType;
				if (varOther != null) {
					return (varOther.precision == precision) && (varOther.scale == scale);
				}
			}
			return false;
		}

		public override int GetHashCode() {
			return base.GetHashCode()^precision^scale*53;
		}
	}
}
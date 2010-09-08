using System;
using System.Diagnostics;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlName: SqlScriptableToken, IEquatable<SqlName>, IComparable<SqlName> {
		private readonly string value;

		protected SqlName(string name) {
			Debug.Assert(!string.IsNullOrEmpty(name));
			value = name;
		}

		public string Value {
			get {
				return value;
			}
		}

		public override sealed void WriteTo(SqlWriter writer) {
			WriteToInternal(writer, false);
		}

		protected internal virtual void WriteToInternal(SqlWriter writer, bool isPartOfQualifiedName) {
			writer.Write(value);
		}

		public override int GetHashCode() {
			return value.GetHashCode()^GetType().GetHashCode();
		}

		public bool Equals(SqlName other) {
			if (other != null) {
				if (ReferenceEquals(this, other)) {
					return true;
				}
				if (GetType() == other.GetType()) {
					return StringComparer.OrdinalIgnoreCase.Equals(value, other.value);
				}
			}
			return false;
		}

		public int CompareTo(SqlName other) {
			if (other == null) {
				return 1;
			}
			if (ReferenceEquals(this, other)) {
				return 0;
			}
			int diff = StringComparer.OrdinalIgnoreCase.Compare(value, other.value);
			if (diff == 0) {
				diff = StringComparer.Ordinal.Compare(GetType().FullName, other.GetType().FullName);
			}
			return diff;
		}
	}
}
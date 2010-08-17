using System;
using System.IO;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlScriptableToken: SqlToken, IEquatable<SqlScriptableToken> {
		private byte[] hash;

		public bool Equals(SqlScriptableToken other) {
			if ((other != null) && (other.GetType() == GetType())) {
				return HashWriter.HashEqual(GetHash(), other.GetHash());
			}
			return false;
		}

		public byte[] GetHash() {
			if (hash == null) {
				using (HashWriter writer = new HashWriter()) {
					WriteTo(new SqlWriter(writer));
					hash = writer.ToArray();
				}
			}
			return hash;
		}

		public void ResetHash() {
			hash = null;
		}

		public override bool Equals(object obj) {
			return base.Equals(obj as SqlScriptableToken);
		}

		public override int GetHashCode() {
			return HashWriter.ToHashCode(GetHash());
		}

		public override sealed string ToString() {
			using (StringWriter writer = new StringWriter()) {
				WriteTo(new SqlWriter(writer));
				return writer.ToString();
			}
		}

		public abstract void WriteTo(SqlWriter writer);
	}
}
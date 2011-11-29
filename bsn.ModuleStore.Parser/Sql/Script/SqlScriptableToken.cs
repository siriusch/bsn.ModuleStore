// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlScriptableToken: SqlToken, IEquatable<SqlScriptableToken>, IEquatable<IHashableStatement>, IHashableStatement {
		private int? hashCode;

		public override bool Equals(object obj) {
			return Equals(obj as IHashableStatement);
		}

		public bool Equals(IHashableStatement other, DatabaseEngine engine) {
			if (other == this) {
				return true;
			}
			if ((other != null) && (other.GetType() == GetType())) {
				return HashWriter.HashEqual(GetHash(engine), other.GetHash(engine));
			}
			return false;
		}

		public override int GetHashCode() {
			if (!hashCode.HasValue) {
				ComputeHashCode();
			}
			return hashCode.GetValueOrDefault();
		}

		public override sealed string ToString() {
			return ToString(DatabaseEngine.Unknown);
		}

		public string ToString(DatabaseEngine engine) {
			using (StringWriter writer = new StringWriter()) {
				WriteTo(new SqlWriter(writer, engine));
				return writer.ToString();
			}
		}

		public abstract void WriteTo(SqlWriter writer);

		internal void ComputeHashCode() {
			hashCode = HashWriter.ToHashCode(GetHash(DatabaseEngine.Unknown));
		}

		public bool Equals(IHashableStatement other) {
			return Equals(other, DatabaseEngine.Unknown);
		}

		bool IEquatable<SqlScriptableToken>.Equals(SqlScriptableToken other) {
			return Equals(other);
		}

		public byte[] GetHash(DatabaseEngine engine) {
			using (HashWriter writer = new HashWriter()) {
				WriteTo(new SqlWriter(writer, engine, false));
				return writer.ToArray();
			}
		}
	}
}

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
//  
using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlScriptableToken: SqlToken, IEquatable<SqlScriptableToken> {
		private byte[] hash;

		public override bool Equals(object obj) {
			return base.Equals(obj as SqlScriptableToken);
		}

		public byte[] GetHash() {
			if (hash == null) {
				hash = GetHashInternal();
			}
			return hash;
		}

		public override int GetHashCode() {
			return HashWriter.ToHashCode(GetHash());
		}

		public void ResetHash() {
			hash = null;
		}

		public override sealed string ToString() {
			using (StringWriter writer = new StringWriter()) {
				WriteTo(new SqlWriter(writer));
				return writer.ToString();
			}
		}

		public abstract void WriteTo(SqlWriter writer);

		protected virtual byte[] GetHashInternal() {
			using (HashWriter writer = new HashWriter()) {
				WriteTo(new SqlWriter(writer, false));
				return writer.ToArray();
			}
		}

		public bool Equals(SqlScriptableToken other) {
			if ((other != null) && (other.GetType() == GetType())) {
				return HashWriter.HashEqual(GetHash(), other.GetHash());
			}
			return false;
		}
	}
}

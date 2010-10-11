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
using System.Diagnostics;

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

		public override int GetHashCode() {
			return value.GetHashCode()^GetType().GetHashCode();
		}

		public override sealed void WriteTo(SqlWriter writer) {
			WriteToInternal(writer, false);
		}

		protected internal virtual void WriteToInternal(SqlWriter writer, bool isPartOfQualifiedName) {
			writer.Write(value);
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
	}
}

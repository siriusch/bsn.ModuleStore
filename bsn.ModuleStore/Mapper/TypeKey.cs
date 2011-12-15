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

namespace bsn.ModuleStore.Mapper {
	// The separate implementations are necessary to make the generics work as expected; this avoids boxing in the Equals() call
	public struct TypeKey<TKey>: ITypeKey<TypeKey<TKey>, TKey> where TKey: IEquatable<TKey> {
		private readonly TKey key;
		private readonly Type type;

		internal TypeKey(Type type, TKey key) {
			this.type = type;
			this.key = key;
		}

		public override int GetHashCode() {
			return type.GetHashCode()^key.GetHashCode();
		}

		public override string ToString() {
			return type.FullName+'['+key+']';
		}

		public TKey Key {
			get {
				return key;
			}
		}

		public Type Type {
			get {
				return type;
			}
		}

		public bool Equals(TypeKey<TKey> other) {
			return key.Equals(other.key) && type.Equals(other.type);
		}
	}

	public struct TypeKey: ITypeKey<TypeKey, object> {
		private readonly object key;
		private readonly Type type;

		internal TypeKey(Type type, object key) {
			this.type = type;
			this.key = key;
		}

		public override int GetHashCode() {
			return type.GetHashCode()^key.GetHashCode();
		}

		public override string ToString() {
			return type.FullName+'['+key+']';
		}

		public object Key {
			get {
				return key;
			}
		}

		public Type Type {
			get {
				return type;
			}
		}

		public bool Equals(TypeKey other) {
			return key.Equals(other.key) && type.Equals(other.type);
		}
	}
}

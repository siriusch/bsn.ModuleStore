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
using System.Collections.Generic;

namespace bsn.ModuleStore.Mapper.Serialization {
	public class SerializationTypeInfoProvider: ISerializationTypeInfoProvider {
		private struct TypeKey: IEquatable<TypeKey> {
			private readonly Type type;
			private readonly bool scalar;

			public TypeKey(Type type, bool scalar) {
				this.type = type;
				this.scalar = scalar;
			}

			public bool Equals(TypeKey other) {
				return type.Equals(other.type) && scalar == other.scalar;
			}

			public override bool Equals(object obj) {
				if (ReferenceEquals(null, obj)) {
					return false;
				}
				return obj is TypeKey && Equals((TypeKey)obj);
			}

			public override int GetHashCode() {
				unchecked {
					return (type.GetHashCode()*397)^scalar.GetHashCode();
				}
			}
		}

		private readonly Dictionary<TypeKey, SerializationTypeInfo> infos = new Dictionary<TypeKey, SerializationTypeInfo>();
		private readonly ISerializationTypeMappingProvider mappingProvider;

		public SerializationTypeInfoProvider(): this(new SerializationTypeMappingProvider()) {}

		public SerializationTypeInfoProvider(ISerializationTypeMappingProvider mappingProvider) {
			if (mappingProvider == null) {
				throw new ArgumentNullException(nameof(mappingProvider));
			}
			this.mappingProvider = mappingProvider;
		}

		public ISerializationTypeInfo GetSerializationTypeInfo(Type type, bool scalar) {
			SerializationTypeInfo result;
			var key = new TypeKey(type, scalar);
			lock (infos) {
				if (!infos.TryGetValue(key, out result)) {
					result = new SerializationTypeInfo(type, scalar, mappingProvider);
					infos.Add(key, result);
				}
			}
			return result;
		}

		public ISerializationTypeMappingProvider TypeMappingProvider => mappingProvider;
	}
}

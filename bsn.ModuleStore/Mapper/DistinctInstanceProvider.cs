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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace bsn.ModuleStore.Mapper {
	public class DistinctInstanceProvider<TKey>: IInstanceProvider, IDeserializationStateProvider where TKey: IEquatable<TKey> {
		protected struct TypeKey: IEquatable<TypeKey> {
			private readonly TKey key;
			private readonly Type type;

			internal TypeKey(Type type, TKey key) {
				this.type = type;
				this.key = key;
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

			public override int GetHashCode() {
				return type.GetHashCode()^key.GetHashCode();
			}

			public override string ToString() {
				return type.FullName+'['+key+']';
			}

			public bool Equals(TypeKey other) {
				return key.Equals(other.key) && type.Equals(other.type);
			}
		}

		protected const string DeserializedInstanceSet = "$DeserializedInstanceSet";

		protected virtual void BeginDeserialize(IDictionary<string, object> state) {
			state[DeserializedInstanceSet] = new Dictionary<TypeKey, object>();
		}

		protected virtual object CreateInstance(TypeKey key) {
			Debug.Assert(!key.Type.IsAbstract);
			return FormatterServices.GetUninitializedObject(key.Type);
		}

		protected virtual object CreateOrGetCachedInstance(TypeKey key, out InstanceOrigin instanceOrigin) {
			instanceOrigin = InstanceOrigin.New;
			return CreateInstance(key);
		}

		protected virtual void EndDeserialize(IDictionary<string, object> state) {
			state.Remove(DeserializedInstanceSet);
		}

		protected virtual void Forget(IDictionary<string, object> state, Type instanceType, object identity) {}

		protected Dictionary<TypeKey, object> GetDeserializedInstances(IDictionary<string, object> state) {
			return (Dictionary<TypeKey, object>)state[DeserializedInstanceSet];
		}

		protected virtual void InstanceDeserialized(IDictionary<string, object> state, object instance) {}

		protected virtual bool TryGetInstance(IDictionary<string, object> state, Type instanceType, object identity, out object instance, out InstanceOrigin instanceOrigin) {
			Debug.Assert(instanceType != null);
			if ((!instanceType.IsValueType) && (identity is TKey)) {
				TypeKey key = new TypeKey(instanceType, (TKey)identity);
				Dictionary<TypeKey, object> deserializedInstances = (state != null) ? GetDeserializedInstances(state) : null;
				if ((deserializedInstances != null) && deserializedInstances.TryGetValue(key, out instance)) {
					instanceOrigin = InstanceOrigin.ResultSet;
				} else {
					instance = CreateOrGetCachedInstance(key, out instanceOrigin);
					if (deserializedInstances != null) {
						deserializedInstances.Add(key, instance);
					}
				}
				return true;
			}
			instance = null;
			instanceOrigin = InstanceOrigin.None;
			return false;
		}

		void IDeserializationStateProvider.BeginDeserialize(IDictionary<string, object> state) {
			BeginDeserialize(state);
		}

		void IDeserializationStateProvider.InstanceDeserialized(IDictionary<string, object> state, object instance) {
			InstanceDeserialized(state, instance);
		}

		void IDeserializationStateProvider.EndDeserialize(IDictionary<string, object> state) {
			EndDeserialize(state);
		}

		bool IInstanceProvider.TryGetInstance(IDictionary<string, object> state, Type instanceType, object identity, out object instance, out InstanceOrigin instanceOrigin) {
			return TryGetInstance(state, instanceType, identity, out instance, out instanceOrigin);
		}

		void IInstanceProvider.ForgetInstance(IDictionary<string, object> state, Type instanceType, object identity) {
			Forget(state, instanceType, identity);
		}
	}
}

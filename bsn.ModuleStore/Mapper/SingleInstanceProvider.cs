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
	public class SingleInstanceProvider<TKey>: IInstanceProvider, IDeserializationStateProvider where TKey: IEquatable<TKey> {
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

			public bool Equals(TypeKey other) {
				return key.Equals(other.key) && type.Equals(other.type);
			}
		}

		private const int GCGeneration = 1;

		private readonly Dictionary<TypeKey, WeakReference> instances = new Dictionary<TypeKey, WeakReference>();
		private int lastCollection;

		void IDeserializationStateProvider.BeginDeserialize(IDictionary<string, object> state) {
			BeginDeserialize(state);
		}

		void IDeserializationStateProvider.EndDeserialize(IDictionary<string, object> state) {
			EndDeserialize(state);
		}

		bool IInstanceProvider.TryGetInstance(Type instanceType, object identity, out object instance) {
			return TryGetInstance(instanceType, identity, out instance);
		}

		protected virtual void BeginDeserialize(IDictionary<string, object> state) {}

		protected virtual void EndDeserialize(IDictionary<string, object> state) {
			// some simple heuristic to determine when to perform a cleanup run
			if (GC.CollectionCount(GCGeneration) > (lastCollection+(int)Math.Log(instances.Count))) {
				lock (instances) {
					List<TypeKey> keys = new List<TypeKey>();
					foreach (KeyValuePair<TypeKey, WeakReference> pair in instances) {
						if (!pair.Value.IsAlive) {
							keys.Add(pair.Key);
						}
					}
					foreach (TypeKey key in keys) {
						instances.Remove(key);
					}
				}
				lastCollection = GC.CollectionCount(GCGeneration);
			}
		}

		protected virtual bool TryGetInstance(Type instanceType, object identity, out object instance) {
			Debug.Assert(instanceType != null);
			if ((!instanceType.IsValueType) && (identity is TKey)) {
				TypeKey key = new TypeKey(instanceType, (TKey)identity);
				lock (instances) {
					WeakReference reference;
					if (instances.TryGetValue(key, out reference)) {
						instance = reference.Target;
						if (instance == null) {
							instance = CreateInstance(key);
							reference.Target = instance;
						}
					} else {
						instance = CreateInstance(key);
						reference = new WeakReference(instance);
						instances.Add(key, reference);
					}
				}
				return true;
			}
			instance = null;
			return false;
		}

		protected virtual object CreateInstance(TypeKey key) {
			return FormatterServices.GetUninitializedObject(key.Type);
		}
	}
}
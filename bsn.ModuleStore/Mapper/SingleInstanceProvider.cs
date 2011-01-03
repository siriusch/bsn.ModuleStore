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
		protected const string DeserializedInstanceSet = "$DeserializedInstanceSet";

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

		protected virtual void BeginDeserialize(IDictionary<string, object> state) {
			state[DeserializedInstanceSet] = new Dictionary<TypeKey, object>();
		}

		protected virtual object CreateInstance(TypeKey key) {
			return FormatterServices.GetUninitializedObject(key.Type);
		}

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

		protected T GetFromCache<T>(TKey identity) where T: class {
			WeakReference reference;
			lock (instances) {
				if (instances.TryGetValue(new TypeKey(typeof(T), identity), out reference)) {
					object result = reference.Target;
					if (result != null) {
						return (T)result;
					}
					throw new KeyNotFoundException(string.Format("The {0} instance with the identity {1} is no longer in the cache", typeof(T).FullName, identity));
				}
			}
			throw new KeyNotFoundException(string.Format("A {0} instance with the identity {1} was not found in the cache", typeof(T).FullName, identity));
		}

		protected virtual void InstanceDeserialized(IDictionary<string, object> state, object instance) {}

		protected bool TryGetFromCache<T>(TKey identity, out T item) where T: class {
			WeakReference reference;
			lock (instances) {
				if (instances.TryGetValue(new TypeKey(typeof(T), identity), out reference)) {
					item = (T)reference.Target;
					return item != null;
				}
			}
			item = null;
			return false;
		}

		protected virtual bool TryGetInstance(IDictionary<string, object> state, Type instanceType, object identity, out object instance, out InstanceOrigin instanceOrigin) {
			Debug.Assert(state != null);
			Debug.Assert(instanceType != null);
			if ((!instanceType.IsValueType) && (identity is TKey)) {
				TypeKey key = new TypeKey(instanceType, (TKey)identity);
				Dictionary<TypeKey, object> deserializedInstances = (Dictionary<TypeKey, object>)state[DeserializedInstanceSet];
				if (deserializedInstances.TryGetValue(key, out instance)) {
					instanceOrigin = InstanceOrigin.ResultSet;
				} else {
					lock (instances) {
						WeakReference reference;
						if (instances.TryGetValue(key, out reference)) {
							instance = reference.Target;
							if (instance == null) {
								instance = CreateInstance(key);
								reference.Target = instance;
								instanceOrigin = InstanceOrigin.New;
							} else {
								instanceOrigin = InstanceOrigin.Cache;
							}
						} else {
							instance = CreateInstance(key);
							reference = new WeakReference(instance);
							instances.Add(key, reference);
							instanceOrigin = InstanceOrigin.New;
						}
						deserializedInstances.Add(key, instance);
					}
				}
				return true;
			}
			instance = null;
			instanceOrigin = InstanceOrigin.None;
			return false;
		}

		protected virtual void Forget(IDictionary<string, object> state, Type instanceType, object identity) {
			Debug.Assert(state != null);
			Debug.Assert(instanceType != null);
			if ((!instanceType.IsValueType) && (identity is TKey)) {
				TypeKey key = new TypeKey(instanceType, (TKey)identity);
				lock (instances) {
					instances.Remove(key);
				}
				((Dictionary<TypeKey, object>)state[DeserializedInstanceSet]).Remove(key);
			}
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
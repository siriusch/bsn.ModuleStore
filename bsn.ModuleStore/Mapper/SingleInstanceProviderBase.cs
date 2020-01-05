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
using System.Diagnostics;
using System.Linq;

namespace bsn.ModuleStore.Mapper {
	public abstract class SingleInstanceProviderBase<TTypeKey, TKey>: DistinctInstanceProviderBase<TTypeKey, TKey> where TTypeKey: struct, ITypeKey<TTypeKey, TKey> {
		private interface ICacheReference {
			bool IsStrong {
				get;
			}

			object Target {
				get;
				set;
			}
		}

		private sealed class StrongCacheReference: ICacheReference {
			public StrongCacheReference(object target) {
				Target = target;
			}

			public object Target {
				get;
				set;
			}

			public bool IsStrong => true;
		}

		private sealed class WeakCacheReference: WeakReference, ICacheReference {
			public WeakCacheReference(object target): base(target) {}

			public bool IsStrong => false;
		}

		internal const CachePolicy DefaultCachePolicy = CachePolicy.WeakReference;
		private const int GCGeneration = 1;
		private static readonly Dictionary<Type, CachePolicy> cachePolicy = new Dictionary<Type, CachePolicy>();

		private readonly CachePolicy defaultCachePolicy;
		private readonly Dictionary<TTypeKey, ICacheReference> instances = new Dictionary<TTypeKey, ICacheReference>();
		private volatile int lastCollection;

		protected SingleInstanceProviderBase(CachePolicy defaultCachePolicy) {
			switch (defaultCachePolicy) {
			case CachePolicy.StrongReference:
			case CachePolicy.WeakReference:
				this.defaultCachePolicy = defaultCachePolicy;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(defaultCachePolicy));
			}
		}

		/// <summary>
		/// Purges the cache by requesting a full GC and then cleaning up the dead references
		/// </summary>
		public void PurgeCache() {
			GC.Collect();
			GC.WaitForPendingFinalizers();
			CleanupCache();
		}

		protected override object CreateOrGetCachedInstance(TTypeKey key, out InstanceOrigin instanceOrigin) {
			object instance;
			lock (instances) {
				if (instances.TryGetValue(key, out var reference)) {
					instance = reference.Target;
					if (instance == null) {
						instance = base.CreateOrGetCachedInstance(key, out instanceOrigin);
						reference.Target = instance;
					} else {
						instanceOrigin = InstanceOrigin.Cache;
					}
				} else {
					instance = base.CreateOrGetCachedInstance(key, out instanceOrigin);
					var policy = GetCachePolicy(key.Type);
					switch (policy) {
					case CachePolicy.WeakReference:
						instances.Add(key, new WeakCacheReference(instance));
						break;
					case CachePolicy.StrongReference:
						instances.Add(key, new StrongCacheReference(instance));
						break;
					}
				}
			}
			return instance;
		}

		protected override void EndDeserialize(IDictionary<string, object> state) {
			// some simple heuristic to determine when to perform a cleanup run
			lock (instances) {
				if (GC.CollectionCount(GCGeneration) > (lastCollection+(int)Math.Log(instances.Count))) {
					CleanupCache();
				}
			}
			base.EndDeserialize(state);
		}

		protected override void Forget(IDictionary<string, object> state, Type instanceType, object identity) {
			Debug.Assert(instanceType != null);
			if ((!instanceType.IsValueType) && (identity is TKey)) {
				var key = CreateTypeKey(instanceType, (TKey)identity);
				lock (instances) {
					instances.Remove(key);
				}
				if (state != null) {
					var deserializedInstances = ((Dictionary<TTypeKey, object>)state[DeserializedInstanceSet]);
					if (deserializedInstances != null) {
						deserializedInstances.Remove(key);
					}
				}
			}
		}

		protected CachePolicy GetCachePolicy(Type type) {
			if (type == null) {
				throw new ArgumentNullException(nameof(type));
			}
			CachePolicy result;
			lock (cachePolicy) {
				if (!cachePolicy.TryGetValue(type, out result)) {
					result = Attribute.GetCustomAttributes(type, typeof(InstanceCacheAttribute)).Cast<InstanceCacheAttribute>().Select(a => a.Policy).SingleOrDefault();
					cachePolicy.Add(type, result);
				}
			}
			return (result == CachePolicy.None) ? defaultCachePolicy : result;
		}

		protected IEnumerable<T> GetFromCache<T>(bool includeWeak) {
			lock (instances) {
				foreach (var cacheReference in instances.Where(p => typeof(T) == p.Key.Type).Select(p => p.Value)) {
					if (includeWeak || cacheReference.IsStrong) {
						var target = cacheReference.Target;
						if (target != null) {
							yield return (T)target;
						}
					}
				}
			}
		}

		protected T GetFromCache<T>(TKey identity) where T: class {
			lock (instances) {
				if (instances.TryGetValue(CreateTypeKey(typeof(T), identity), out var reference)) {
					var result = reference.Target;
					if (result != null) {
						return (T)result;
					}
					throw new KeyNotFoundException($"The {typeof(T).FullName} instance with the identity {identity} is no longer in the cache");
				}
			}
			throw new KeyNotFoundException($"A {typeof(T).FullName} instance with the identity {identity} was not found in the cache");
		}

		protected bool TryGetFromCache<T>(TKey identity, out T item) where T: class {
			lock (instances) {
				if (instances.TryGetValue(CreateTypeKey(typeof(T), identity), out var reference)) {
					item = (T)reference.Target;
					return item != null;
				}
			}
			item = null;
			return false;
		}

		private void CleanupCache() {
			lock (instances) {
				var keys = new List<TTypeKey>();
				foreach (var pair in instances) {
					if (pair.Value.Target == null) {
						keys.Add(pair.Key);
					}
				}
				foreach (var key in keys) {
					instances.Remove(key);
				}
			}
			lastCollection = GC.CollectionCount(GCGeneration);
		}
	}
}
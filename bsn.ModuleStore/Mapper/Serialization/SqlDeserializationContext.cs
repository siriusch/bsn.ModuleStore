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
using System.Text;

namespace bsn.ModuleStore.Mapper.Serialization {
	public class SqlDeserializationContext: IDisposable {
		private readonly Dictionary<object, bool> deserialized = new Dictionary<object, bool>(ReferenceEqualityComparer<object>.Default);
		private readonly IInstanceProvider provider;
		private readonly ISerializationTypeInfoProvider typeInfoProvider;
		private readonly IDictionary<string, object> state;
		private readonly IDeserializationStateProvider stateProvider;

		internal SqlDeserializationContext(IInstanceProvider provider, ISerializationTypeInfoProvider typeInfoProvider) {
			this.provider = provider;
			this.typeInfoProvider = typeInfoProvider;
			stateProvider = provider as IDeserializationStateProvider;
			if (stateProvider != null) {
				state = new SortedDictionary<string, object>(StringComparer.Ordinal);
				stateProvider.BeginDeserialize(state);
			}
		}

		public ISerializationTypeInfo GetSerializationTypeInfo(Type type) {
			return typeInfoProvider.GetSerializationTypeInfo(type);
		}

		public void AssertDeserialization(object instance) {
			if (!deserialized.ContainsKey(instance)) {
				deserialized.Add(instance, false);
			}
		}

		public void ForgetInstance(Type type, object identity) {
			if (provider != null) {
				provider.ForgetInstance(state, type, identity);
			}
		}

		public bool IsDeserialized(object obj) {
			bool result;
			return deserialized.TryGetValue(obj, out result) && result;
		}

		public bool TryGetInstance(Type instanceType, object identity, out object result, out InstanceOrigin instanceOrigin) {
			if (provider != null) {
				return provider.TryGetInstance(state, instanceType, identity, out result, out instanceOrigin);
			}
			result = null;
			instanceOrigin = InstanceOrigin.None;
			return false;
		}

		internal void NotifyInstancePopulated(object instance) {
			Debug.Assert((instance != null) && (!instance.GetType().IsValueType));
			if (stateProvider != null) {
				stateProvider.InstanceDeserialized(state, instance);
			}
			deserialized[instance] = true;
		}

		void IDisposable.Dispose() {
			if (stateProvider != null) {
				stateProvider.EndDeserialize(state);
			}
			if (deserialized.ContainsValue(false)) {
				StringBuilder error = new StringBuilder("Some objects were created as forward reference but were missing in the result set");
				foreach (KeyValuePair<Type, int> pair in deserialized.Where(p => !p.Value).Select(p => p.Key).GroupBy(o => o.GetType(), (t, o) => new KeyValuePair<Type, int>(t, o.Count()))) {
					error.AppendLine();
					error.AppendFormat("Found {0} unresolved forward references of the type {1} in resultset", pair.Value, pair.Key);
				}
				throw new InvalidOperationException(error.ToString());
			}
		}
	}
}

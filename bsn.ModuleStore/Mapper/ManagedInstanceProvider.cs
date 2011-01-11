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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace bsn.ModuleStore.Mapper {
	public class ManagedInstanceProvider<TId, TManager>: SingleInstanceProvider<TId> where TId: struct, IEquatable<TId> where TManager: InstanceManager<TId, TManager> {
		private static readonly Dictionary<Type, DynamicMethod> factoryMethods = new Dictionary<Type, DynamicMethod>();

		public static Func<Instance<TId, TManager>> CreateFactory(Type type, TManager manager) {
			Debug.Assert(typeof(Instance<TId, TManager>).IsAssignableFrom(type));
			Debug.Assert(manager != null);
			DynamicMethod factoryMethod;
			lock (factoryMethods) {
				if (!factoryMethods.TryGetValue(type, out factoryMethod)) {
					Type[] arguments = new[] {typeof(TManager)};
					ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic, null, arguments, null);
					if (constructor == null) {
						throw new MissingMemberException(type.FullName, string.Format(".ctor({0})", typeof(TManager).FullName));
					}
					factoryMethod = new DynamicMethod(string.Format("ManagedInstanceFactory<{0}>", type.Name), type, arguments, type, true);
					ILGenerator il = factoryMethod.GetILGenerator();
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Newobj, constructor);
					il.Emit(OpCodes.Ret);
					factoryMethods.Add(type, factoryMethod);
				}
			}
			return (Func<Instance<TId, TManager>>)factoryMethod.CreateDelegate(typeof(Func<Instance<TId, TManager>>), manager);
		}

		private readonly Dictionary<Type, Func<Instance<TId, TManager>>> factories = new Dictionary<Type, Func<Instance<TId, TManager>>>();
		private TManager manager;

		protected internal TManager Manager {
			get {
				return manager;
			}
			internal set {
				if (value == null) {
					throw new ArgumentNullException("manager");
				}
				if (manager != null) {
					throw new InvalidOperationException("Manager can only be set once");
				}
				manager = value;
			}
		}

		protected override object CreateInstance(TypeKey key) {
			Debug.Assert(manager != null);
			if (typeof(Instance<TId, TManager>).IsAssignableFrom(key.Type)) {
				Func<Instance<TId, TManager>> factory;
				lock (factories) {
					if (!factories.TryGetValue(key.Type, out factory)) {
						factory = CreateFactory(key.Type, manager);
						factories.Add(key.Type, factory);
					}
				}
				return factory();
			}
			return base.CreateInstance(key);
		}
	}
}
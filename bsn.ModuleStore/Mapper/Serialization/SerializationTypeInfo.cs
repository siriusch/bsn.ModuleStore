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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

using Common.Logging;

namespace bsn.ModuleStore.Mapper.Serialization {
	public class SerializationTypeInfo: ISerializationTypeInfo {
		private static class ToArray<T> {
#pragma warning disable 169
			// ReSharper disable StaticFieldInGenericType
			// this field is used via reflection
			public static readonly Func<object, Array> ToArrayInvoker = CreateToArrayInvoker();
			// ReSharper restore StaticFieldInGenericType
#pragma warning restore 169

			private static Func<object, Array> CreateToArrayInvoker() {
				MethodInfo toArrayMethod = typeof(T).GetMethod("ToArray");
				if (toArrayMethod == null) {
					return null;
				}
				DynamicMethod method = new DynamicMethod(string.Format("{0}.ToArray`Invoke", typeof(T).FullName), typeof(Array), new[] {typeof(object)}, false);
				ILGenerator il = method.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Castclass, typeof(T));
				il.Emit(OpCodes.Callvirt, toArrayMethod);
				il.Emit(OpCodes.Ret);
				return (Func<object, Array>)method.CreateDelegate(typeof(Func<object, Array>));
			}
		}

		private static readonly ILog log = LogManager.GetLogger<SerializationTypeInfo>();

		// ReSharper disable UnusedMember.Local
		public static Array ToArrayGeneric<T>(IEnumerable enumerable) {
			// ReSharper restore UnusedMember.Local
			List<T> list = new List<T>();
			foreach (object obj in enumerable) {
				list.Add((T)obj);
			}
			return list.ToArray();
		}

		private readonly Type instanceType;
		private readonly bool isXmlType;
		private readonly Func<object, Array> listToArray;
		private readonly Type listType;
		private readonly ISerializationTypeMapping mapping;
		private readonly bool requiresNotification;
		private readonly IMemberConverter simpleConverter;
		private readonly Type type;

		public SerializationTypeInfo(Type type, ISerializationTypeMappingProvider typeMappingProvider) {
			this.type = type;
			if (type.IsArray) {
				if (type.GetArrayRank() != 1) {
					throw new NotSupportedException("Only arrays with one dimension are supported by the DbDeserializer");
				}
				instanceType = type.GetElementType();
				Debug.Assert(instanceType != null);
			} else if ((type == typeof(string)) || (!type.TryGetIEnumerableElementType(out instanceType))) {
				instanceType = type;
			}
			if (instanceType.IsArray) {
				throw new NotSupportedException("Nested arrays cannot be deserialized by the DbDeserializer");
			}
			requiresNotification = typeof(ISqlDeserializationHook).IsAssignableFrom(instanceType);
			mapping = typeMappingProvider.GetMapping(instanceType);
			if (IsCollection) {
				listType = (type.IsInterface || type.IsArray) ? typeof(List<>).MakeGenericType(instanceType) : type;
				if (type.IsArray) {
					// ReSharper disable PossibleNullReferenceException
					// ReSharper disable AssignNullToNotNullAttribute
					listToArray = (Func<object, Array>)typeof(ToArray<>).MakeGenericType(listType).GetField("ToArrayInvoker", BindingFlags.Public|BindingFlags.Static).GetValue(null);
					// ReSharper restore AssignNullToNotNullAttribute
					// ReSharper restore PossibleNullReferenceException
					if (listToArray == null) {
						listToArray = (Func<object, Array>)Delegate.CreateDelegate(typeof(Func<object, Array>), typeof(SerializationTypeInfo).GetMethod("ToArrayGeneric").MakeGenericMethod(instanceType));
					}
				}
			}
			isXmlType = instanceType.IsXmlType();
			if (isXmlType || (Nullable.GetUnderlyingType(instanceType) != null) || instanceType.IsPrimitive || mapping.IsNativeType) {
				simpleConverter = MemberConverter.Get(instanceType, false, null, 0, DateTimeKind.Unspecified);
			}
		}

		public Type InstanceType {
			get {
				return instanceType;
			}
		}

		public bool IsCollection {
			get {
				return type != instanceType;
			}
		}

		public bool IsXmlInstanceType {
			get {
				return isXmlType;
			}
		}

		public Type ListType {
			get {
				return listType;
			}
		}

		public bool RequiresNotification {
			get {
				return requiresNotification;
			}
		}

		public Type Type {
			get {
				return type;
			}
		}

		public ISerializationTypeMapping Mapping {
			get {
				return mapping;
			}
		}

		public IMemberConverter SimpleConverter {
			get {
				return simpleConverter;
			}
		}

		public IList CreateList() {
			try {
				return (IList)Activator.CreateInstance(listType);
			} catch {
				log.WarnFormat("Failed to create list of type {0}", listType.FullName);
				throw;
			}
		}

		public object FinalizeList(IList list) {
			if ((list != null) && (listToArray != null)) {
				return listToArray(list);
			}
			return list;
		}
	}
}

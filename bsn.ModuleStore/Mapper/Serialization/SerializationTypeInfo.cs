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

using NLog;

namespace bsn.ModuleStore.Mapper.Serialization {
	public class SerializationTypeInfo: ISerializationTypeInfo {
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private static class ToArray<T> {
#pragma warning disable 169
			// ReSharper disable StaticFieldInGenericType
			// this field is used via reflection
			public static readonly Func<object, Array> ToArrayInvoker = CreateToArrayInvoker();
			// ReSharper restore StaticFieldInGenericType
#pragma warning restore 169

			private static Func<object, Array> CreateToArrayInvoker() {
				var toArrayMethod = typeof(T).GetMethod("ToArray");
				if (toArrayMethod == null) {
					return null;
				}
				var method = new DynamicMethod($"{typeof(T).FullName}.ToArray`Invoke", typeof(Array), new[] {typeof(object)}, false);
				var il = method.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Castclass, typeof(T));
				il.Emit(OpCodes.Callvirt, toArrayMethod);
				il.Emit(OpCodes.Ret);
				return (Func<object, Array>)method.CreateDelegate(typeof(Func<object, Array>));
			}
		}

		// ReSharper disable UnusedMember.Local
		public static Array ToArrayGeneric<T>(IEnumerable enumerable) {
			// ReSharper restore UnusedMember.Local
			var list = new List<T>();
			foreach (var obj in enumerable) {
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

		public SerializationTypeInfo(Type type, bool scalar, ISerializationTypeMappingProvider typeMappingProvider) {
			this.type = type;
			if (scalar) {
				instanceType = type;
			} else {
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

		public Type InstanceType => instanceType;

		public bool IsCollection => type != instanceType;

		public bool IsXmlInstanceType => isXmlType;

		public Type ListType => listType;

		public bool RequiresNotification => requiresNotification;

		public Type Type => type;

		public ISerializationTypeMapping Mapping => mapping;

		public IMemberConverter SimpleConverter => simpleConverter;

		public IList CreateList() {
			try {
				return (IList)Activator.CreateInstance(listType);
			} catch {
				log.Warn("Failed to create list of type {listType}", listType.FullName);
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

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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace bsn.ModuleStore.Mapper.Serialization {
	public class SqlSerializationTypeInfo {
		private static class ToArray<T> {
#pragma warning disable 169
			// this field is used via reflection
			public static readonly Func<object, Array> ToArrayInvoker = CreateToArrayInvoker();
#pragma warning restore 169

			private static Func<object, Array> CreateToArrayInvoker() {
				MethodInfo toArrayMethod = typeof(T).GetMethod("ToArray");
				if (toArrayMethod == null) {
					return null;
				}
				DynamicMethod method = new DynamicMethod(string.Format("{0}.ToArray`Invoke", typeof(T).FullName), typeof(Array), new[] { typeof(object) }, false);
				ILGenerator il = method.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Castclass, typeof(T));
				il.Emit(OpCodes.Callvirt, toArrayMethod);
				il.Emit(OpCodes.Ret);
				return (Func<object, Array>)method.CreateDelegate(typeof(Func<object, Array>));
			}
		}

		private static readonly Dictionary<Type, SqlSerializationTypeInfo> infos = new Dictionary<Type, SqlSerializationTypeInfo>();

// ReSharper disable UnusedMember.Local
		private static Array ToArrayGeneric<T>(IEnumerable enumerable) {
// ReSharper restore UnusedMember.Local
			List<T> list = new List<T>();
			foreach (object obj in enumerable) {
				list.Add((T)obj);
			}
			return list.ToArray();
		}

		public static SqlSerializationTypeInfo Get(Type type) {
			SqlSerializationTypeInfo result;
			lock (infos) {
				if (!infos.TryGetValue(type, out result)) {
					result = new SqlSerializationTypeInfo(type);
					infos.Add(type, result);
				}
			}
			return result;
		}

		private static Type GetElementTypeOfIEnumerable(Type type) {
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
				return type.GetGenericArguments()[0];
			}
			return null;
		}

		public static bool TryGetIEnumerableElementType(Type type, out Type elementType) {
			elementType = GetElementTypeOfIEnumerable(type);
			if (elementType != null) {
				return true;
			}
			foreach (Type interfaceType in type.GetInterfaces()) {
				elementType = GetElementTypeOfIEnumerable(interfaceType);
				if (elementType != null) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns true if the type given is a known XML type.
		/// </summary>
		/// <param name="type">Te type to be checked.</param>
		/// <returns>True if the type is recognized as XML type.</returns>
		public static bool IsXmlType(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			return typeof(XContainer).IsAssignableFrom(type) || typeof(XmlReader).IsAssignableFrom(type) || typeof(XPathNavigator).IsAssignableFrom(type) || typeof(IXPathNavigable).IsAssignableFrom(type);
		}

		private readonly Type instanceType;
		private readonly bool isXmlType;
		private readonly Func<object, Array> listToArray;
		private readonly Type listType;
		private readonly SqlSerializationTypeMapping mapping;
		private readonly bool requiresNotification;
		private readonly MemberConverter simpleConverter;
		private readonly Type type;

		private SqlSerializationTypeInfo(Type type) {
			this.type = type;
			if (type.IsArray) {
				if (type.GetArrayRank() != 1) {
					throw new NotSupportedException("Only arrays with one dimension are supported by the DbDeserializer");
				}
				instanceType = type.GetElementType();
				Debug.Assert(instanceType != null);
			} else if ((type == typeof(string)) || (!TryGetIEnumerableElementType(type, out instanceType))) {
				instanceType = type;
			}
			if (instanceType.IsArray) {
				throw new NotSupportedException("Nested arrays cannot be deserialized by the DbDeserializer");
			}
			requiresNotification = typeof(ISqlDeserializationHook).IsAssignableFrom(instanceType);
			mapping = SqlSerializationTypeMapping.Get(instanceType);
			if (IsCollection) {
				listType = (type.IsInterface || type.IsArray) ? typeof(List<>).MakeGenericType(instanceType) : type;
				if (type.IsArray) {
// ReSharper disable PossibleNullReferenceException
// ReSharper disable AssignNullToNotNullAttribute
					listToArray = (Func<object, Array>)typeof(ToArray<>).MakeGenericType(listType).GetField("ToArrayInvoker", BindingFlags.Public|BindingFlags.Static).GetValue(null);
// ReSharper restore AssignNullToNotNullAttribute
// ReSharper restore PossibleNullReferenceException
					if (listToArray == null) {
						listToArray = (Func<object, Array>)Delegate.CreateDelegate(typeof(Func<object, Array>), typeof(SqlSerializationTypeInfo).GetMethod("ToArrayGeneric").MakeGenericMethod(instanceType));
					}
				}
			}
			isXmlType = IsXmlType(instanceType);
			if (isXmlType || (Nullable.GetUnderlyingType(instanceType) != null) || instanceType.IsPrimitive || SqlSerializationTypeMapping.IsNativeType(instanceType)) {
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

		internal SqlSerializationTypeMapping Mapping {
			get {
				return mapping;
			}
		}

		internal MemberConverter SimpleConverter {
			get {
				return simpleConverter;
			}
		}

		public IList CreateList() {
			try {
				return (IList)Activator.CreateInstance(listType);
			} catch {
				Debug.WriteLine(listType, "Failed to create list");
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

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
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace bsn.ModuleStore.Mapper.Serialization {
	public class SqlSerializationTypeInfo {
		private static readonly Dictionary<Type, SqlSerializationTypeInfo> infos = new Dictionary<Type, SqlSerializationTypeInfo>();

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
		private readonly MethodInfo listToArray;
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
			} else if (!TryGetIEnumerableElementType(type, out instanceType)) {
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
					listToArray = listType.GetMethod("ToArray");
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
				return listToArray.Invoke(list, null);
			}
			return list;
		}
	}
}

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
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace bsn.ModuleStore.Mapper {
	internal static class TypeExtensionMethods {
		/// <summary>
		/// Returns true if the type given is a nullable type (that is, <see cref="Nullable{T}"/>).
		/// </summary>
		/// <param name="type">The type to be checked.</param>
		/// <returns>True if the type is recognized as nullable type.</returns>
		public static bool IsNullableType(this Type type)
		{
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		public static IEnumerable<MemberInfo> GetAllFieldsAndProperties(this Type type) {
			while (type != null) {
				foreach (FieldInfo field in type.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly)) {
					yield return field;
				}
				foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly)) {
					yield return property;
				}
				type = type.BaseType;
			}
		}

		private static Type GetElementTypeOfIEnumerable(Type type) {
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
				return type.GetGenericArguments()[0];
			}
			return null;
		}

		/// <summary>
		/// Returns true if the type given is a known XML type.
		/// </summary>
		/// <param name="type">Te type to be checked.</param>
		/// <returns>True if the type is recognized as XML type.</returns>
		public static bool IsXmlType(this Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			return typeof(XContainer).IsAssignableFrom(type) || typeof(XmlReader).IsAssignableFrom(type) || typeof(XPathNavigator).IsAssignableFrom(type) || typeof(IXPathNavigable).IsAssignableFrom(type);
		}

		public static bool TryGetIEnumerableElementType(this Type type, out Type elementType) {
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
	}
}

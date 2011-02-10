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

namespace bsn.ModuleStore.Mapper.Deserialization {
	public class SqlDeserializerTypeInfo {
		private static readonly Dictionary<Type, SqlDeserializerTypeInfo> infos = new Dictionary<Type, SqlDeserializerTypeInfo>();

		public static SqlDeserializerTypeInfo Get(Type type) {
			SqlDeserializerTypeInfo result;
			lock (infos) {
				if (!infos.TryGetValue(type, out result)) {
					result = new SqlDeserializerTypeInfo(type);
					infos.Add(type, result);
				}
			}
			return result;
		}

		private readonly Type instanceType;
		private readonly bool isXmlType;
		private readonly MethodInfo listToArray;
		private readonly Type listType;
		private readonly SqlDeserializerTypeMapping mapping;
		private readonly bool requiresNotification;
		private readonly MemberConverter simpleConverter;
		private readonly Type type;

		private SqlDeserializerTypeInfo(Type type) {
			this.type = type;
			if (type.IsArray) {
				if (type.GetArrayRank() != 1) {
					throw new NotSupportedException("Only arrays with one dimension are supported by the DbDeserializer");
				}
				instanceType = type.GetElementType();
				Debug.Assert(instanceType != null);
			} else if (type.IsGenericType && ((type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || (type.GetGenericTypeDefinition() == typeof(ICollection<>)) || (type.GetGenericTypeDefinition() == typeof(IList<>)) || (type.GetGenericTypeDefinition() == typeof(List<>)))) {
				instanceType = type.GetGenericArguments()[0];
			} else {
				instanceType = type;
			}
			if (instanceType.IsArray) {
				throw new NotSupportedException("Nested arrays cannot be deserialized by the DbDeserializer");
			}
			requiresNotification = typeof(ISqlDeserializationHook).IsAssignableFrom(instanceType);
			mapping = SqlDeserializerTypeMapping.Get(instanceType);
			if (IsCollection) {
				listType = typeof(List<>).MakeGenericType(instanceType);
				if (type.IsArray) {
					listToArray = listType.GetMethod("ToArray");
				}
			}
			isXmlType = SqlDeserializer.IsXmlType(instanceType);
			if (isXmlType || (Nullable.GetUnderlyingType(instanceType) != null) || instanceType.IsPrimitive || SqlCallProcedureInfo.IsNativeType(instanceType)) {
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

		internal SqlDeserializerTypeMapping Mapping {
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
			return (IList)Activator.CreateInstance(listType);
		}

		public object FinalizeList(IList list) {
			if ((list != null) && (listToArray != null)) {
				return listToArray.Invoke(list, null);
			}
			return list;
		}
	}
}

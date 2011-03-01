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
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

using Microsoft.SqlServer.Server;

namespace bsn.ModuleStore.Mapper.Serialization {
	internal class SqlSerializationTypeMapping {
		private static readonly Dictionary<Type, SqlDbType> dbTypeMapping = new Dictionary<Type, SqlDbType> {
		                                                                                                    		{typeof(long), SqlDbType.BigInt},
		                                                                                                    		{typeof(byte[]), SqlDbType.VarBinary},
		                                                                                                    		{typeof(bool), SqlDbType.Bit},
		                                                                                                    		{typeof(char), SqlDbType.NChar},
		                                                                                                    		{typeof(char[]), SqlDbType.NVarChar},
		                                                                                                    		{typeof(string), SqlDbType.NVarChar},
		                                                                                                    		{typeof(DateTime), SqlDbType.DateTime},
		                                                                                                    		{typeof(DateTimeOffset), SqlDbType.DateTimeOffset},
		                                                                                                    		{typeof(decimal), SqlDbType.Decimal},
		                                                                                                    		{typeof(float), SqlDbType.Real},
		                                                                                                    		{typeof(double), SqlDbType.Float},
		                                                                                                    		{typeof(int), SqlDbType.Int},
		                                                                                                    		{typeof(short), SqlDbType.SmallInt},
		                                                                                                    		{typeof(sbyte), SqlDbType.TinyInt},
		                                                                                                    		{typeof(Guid), SqlDbType.UniqueIdentifier}
		                                                                                                    };

		private static readonly Dictionary<Type, SqlSerializationTypeMapping> mappings = new Dictionary<Type, SqlSerializationTypeMapping>();

		internal static bool IsNativeType(Type type) {
			return dbTypeMapping.ContainsKey(type);
		}

		/// <summary>
		/// Get the matching <see cref="DbType"/> for the given type.
		/// </summary>
		/// <param name="type">The .NET type to match.</param>
		/// <returns>The <see cref="DbType"/> best matching the given .NET type, or <see cref="DbType.Object"/> otherwise.</returns>
		public static SqlDbType GetTypeMapping(Type type) {
			if (type != null) {
				if (type.IsByRef && type.HasElementType) {
					type = type.GetElementType();
					Debug.Assert(type != null);
				} else {
					type = Nullable.GetUnderlyingType(type) ?? type;
				}
				SqlDbType result;
				if (dbTypeMapping.TryGetValue(type, out result)) {
					return result;
				}
				if (SqlSerializationTypeInfo.IsXmlType(type)) {
					return SqlDbType.Xml;
				}
				Type dummyType;
				if (SqlSerializationTypeInfo.TryGetIEnumerableElementType(type, out dummyType)) {
					return SqlDbType.Structured;
				}
				if (type.IsDefined(typeof(SqlUserDefinedTypeAttribute), false)) {
					return SqlDbType.Udt;
				}
			}
			return SqlDbType.Variant;
		}

		public static SqlSerializationTypeMapping Get(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			SqlSerializationTypeMapping result;
			lock (mappings) {
				if (!mappings.TryGetValue(type, out result)) {
					result = new SqlSerializationTypeMapping(type);
					mappings.Add(type, result);
				}
			}
			return result;
		}

		internal static IEnumerable<FieldInfo> GetAllFields(Type type) {
			while (type != null) {
				foreach (FieldInfo field in type.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly)) {
					yield return field;
				}
				type = type.BaseType;
			}
		}

		private readonly Dictionary<string, SqlColumnInfo> columns = new Dictionary<string, SqlColumnInfo>(StringComparer.OrdinalIgnoreCase);
		private readonly ReadOnlyCollection<MemberConverter> converters;
		private readonly bool hasNestedSerializers;
		private readonly FieldInfo[] members;

		private SqlSerializationTypeMapping(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			List<MemberConverter> memberConverters = new List<MemberConverter>();
			List<FieldInfo> memberInfos = new List<FieldInfo>();
			if (!(type.IsPrimitive || type.IsInterface || (typeof(string) == type))) {
				bool hasIdentity = false;
				foreach (FieldInfo field in GetAllFields(type)) {
					SqlColumnAttribute columnAttribute = SqlColumnAttribute.GetColumnAttribute(field, false);
					if (columnAttribute != null) {
						bool isIdentity = (!hasIdentity) && (hasIdentity |= columnAttribute.Identity);
						MemberConverter memberConverter;
						if (columnAttribute.GetCachedByIdentity) {
							memberConverter = new CachedMemberConverter(field.FieldType, isIdentity, columnAttribute.Name, memberInfos.Count, columnAttribute.DateTimeKind);
						} else {
							memberConverter = MemberConverter.Get(field.FieldType, isIdentity, columnAttribute.Name, memberInfos.Count, columnAttribute.DateTimeKind);
						}
						memberConverters.Add(memberConverter);
						memberInfos.Add(field);
						columns.Add(columnAttribute.Name, new SqlColumnInfo(field, columnAttribute.Name, memberConverter));
					} else if (field.IsDefined(typeof(SqlDeserializeAttribute), true)) {
						NestedMemberConverter nestedMemberConverter;
						// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
						if (typeof(IList).IsAssignableFrom(field.FieldType)) {
							nestedMemberConverter = new NestedListMemberConverter(field.FieldType, memberInfos.Count);
						} else {
							nestedMemberConverter = new NestedMemberConverter(field.FieldType, memberInfos.Count);
						}
						// ReSharper restore ConvertIfStatementToConditionalTernaryExpression
						memberInfos.Add(field);
						memberConverters.Add(nestedMemberConverter);
						hasNestedSerializers = true;
#warning add support for table valued parameters and SqlDeserializeAttribute (flatten the structure to one "table")
					}
				}
			}
			members = memberInfos.ToArray();
			converters = Array.AsReadOnly(memberConverters.ToArray());
		}

		public IDictionary<string, SqlColumnInfo> Columns {
			get {
				return columns;
			}
		}

		public ReadOnlyCollection<MemberConverter> Converters {
			get {
				return converters;
			}
		}

		public bool HasNestedSerializers {
			get {
				return hasNestedSerializers;
			}
		}

		public int MemberCount {
			get {
				return members.Length;
			}
		}

		public object GetMember(object instance, int index) {
			return members[index].GetValue(instance);
		}

		public void PopulateInstanceMembers(object result, object[] buffer) {
			FormatterServices.PopulateObjectMembers(result, members, buffer);
		}
	}
}

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
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;

using Microsoft.SqlServer.Server;

namespace bsn.ModuleStore.Mapper.Serialization {
	public class SerializationTypeMapping: ISerializationTypeMapping {
		// the boolean specifies whether this is a native type or not
		private static readonly Dictionary<Type, KeyValuePair<SqlDbType, bool>> dbTypeMapping = new Dictionary<Type, KeyValuePair<SqlDbType, bool>> {
				{typeof(long), new KeyValuePair<SqlDbType, bool>(SqlDbType.BigInt, true)},
				{typeof(byte[]), new KeyValuePair<SqlDbType, bool>(SqlDbType.VarBinary, true)},
				{typeof(bool), new KeyValuePair<SqlDbType, bool>(SqlDbType.Bit, true)},
				{typeof(char), new KeyValuePair<SqlDbType, bool>(SqlDbType.NChar, true)},
				{typeof(char[]), new KeyValuePair<SqlDbType, bool>(SqlDbType.NVarChar, true)},
				{typeof(string), new KeyValuePair<SqlDbType, bool>(SqlDbType.NVarChar, true)},
				{typeof(DateTime), new KeyValuePair<SqlDbType, bool>(SqlDbType.DateTime, true)},
				{typeof(DateTimeOffset), new KeyValuePair<SqlDbType, bool>(SqlDbType.DateTimeOffset, true)},
				{typeof(decimal), new KeyValuePair<SqlDbType, bool>(SqlDbType.Decimal, true)},
				{typeof(float), new KeyValuePair<SqlDbType, bool>(SqlDbType.Real, true)},
				{typeof(double), new KeyValuePair<SqlDbType, bool>(SqlDbType.Float, true)},
				{typeof(int), new KeyValuePair<SqlDbType, bool>(SqlDbType.Int, true)},
				{typeof(short), new KeyValuePair<SqlDbType, bool>(SqlDbType.SmallInt, true)},
				{typeof(byte), new KeyValuePair<SqlDbType, bool>(SqlDbType.TinyInt, true)},
				{typeof(Guid), new KeyValuePair<SqlDbType, bool>(SqlDbType.UniqueIdentifier, true)}
		};

		private static bool CheckNativeType(Type type) {
			lock (dbTypeMapping) {
				return dbTypeMapping.TryGetValue(type, out var dbType) && dbType.Value;
			}
		}

		private static SqlDbType GetTypeMapping(Type type) {
			if (type != null) {
				if (type.IsByRef && type.HasElementType) {
					type = type.GetElementType();
					Debug.Assert(type != null);
				} else {
					type = Nullable.GetUnderlyingType(type) ?? type;
				}
				if (type.IsEnum) {
					type = Enum.GetUnderlyingType(type);
				}
				lock (dbTypeMapping) {
					if (dbTypeMapping.TryGetValue(type, out var mapping)) {
						return mapping.Key;
					}
					SqlDbType result;
					if (type.IsDefined(typeof(SqlUserDefinedTypeAttribute), false)) {
						result = SqlDbType.Udt;
					} else {
						if (type.IsXmlType()) {
							result = SqlDbType.Xml;
						} else {
							if (type.TryGetIEnumerableElementType(out var dummyType)) {
								result = SqlDbType.Structured;
							} else {
								result = SqlDbType.Variant;
							}
						}
					}
					dbTypeMapping.Add(type, new KeyValuePair<SqlDbType, bool>(result, result == SqlDbType.Xml));
					return result;
				}
			}
			return SqlDbType.Variant;
		}

		private readonly Dictionary<string, SqlColumnInfo> columns = new Dictionary<string, SqlColumnInfo>(StringComparer.OrdinalIgnoreCase);
		private readonly ReadOnlyCollection<IMemberConverter> converters;
		private readonly SqlDbType dbType;
		private readonly bool hasNestedSerializers;
		private readonly bool isNativeType;
		private readonly MemberInfo[] members;
		private readonly MembersMethods methods;

		public SerializationTypeMapping(Type type, ISerializationTypeMappingProvider typeMappingProvider) {
			if (type == null) {
				throw new ArgumentNullException(nameof(type));
			}
			// required to enable recursive resolution of mappings
			typeMappingProvider.RegisterMapping(type, this);
			var memberConverters = new List<IMemberConverter>();
			var memberInfos = new List<MemberInfo>();
			isNativeType = CheckNativeType(type);
			dbType = GetTypeMapping(type);
			if (!(type.IsPrimitive || type.IsInterface || (typeof(string) == type))) {
				var hasIdentity = false;
				foreach (var member in type.GetAllFieldsAndProperties()) {
					var columnAttribute = SqlColumnAttributeBase.Get<SqlColumnAttribute>(member, false);
					var memberType = member.GetMemberType();
					if (columnAttribute != null) {
						AssertValidMember(member);
						var isIdentity = (!hasIdentity) && (hasIdentity |= columnAttribute.Identity);
						IMemberConverter memberConverter;
						if (columnAttribute.GetCachedByIdentity) {
							memberConverter = new CachedMemberConverter(memberType, isIdentity, columnAttribute.Name, memberInfos.Count, columnAttribute.DateTimeKind);
						} else {
							memberConverter = MemberConverter.Get(memberType, isIdentity, columnAttribute.Name, memberInfos.Count, columnAttribute.DateTimeKind);
						}
						memberConverters.Add(memberConverter);
						memberInfos.Add(member);
						columns.Add(columnAttribute.Name, new SqlColumnInfo(member, columnAttribute.Name, memberConverter, typeMappingProvider.GetMapping(memberType)));
					} else if (member.IsDefined(typeof(SqlDeserializeAttribute), true)) {
						AssertValidMember(member);
						NestedMemberConverter nestedMemberConverter;
						if (typeof(IList).IsAssignableFrom(memberType)) {
							nestedMemberConverter = new NestedListMemberConverter(memberType, memberInfos.Count);
						} else {
							nestedMemberConverter = new NestedMemberConverter(memberType, memberInfos.Count);
						}
						memberConverters.Add(nestedMemberConverter);
						memberInfos.Add(member);
						hasNestedSerializers = true;
#warning add support for table valued parameters and SqlDeserializeAttribute (flatten the structure to one "table")
					}
				}
			}
			members = memberInfos.ToArray();
			converters = Array.AsReadOnly(memberConverters.ToArray());
			methods = MembersMethods.Get(members);
		}

		private void AssertValidMember(MemberInfo memberInfo) {
			var declaringType = memberInfo.DeclaringType;
			Debug.Assert(declaringType != null);
			switch (memberInfo) {
			case FieldInfo fieldInfo:
				if (fieldInfo.IsInitOnly) {
					throw new InvalidOperationException($"The field {declaringType.FullName}.{fieldInfo.Name} cannot be used as SQL column because it is readonly");
				}
				break;
			case PropertyInfo propertyInfo when !declaringType.IsClass:
				throw new InvalidOperationException($"The property {declaringType.FullName}.{propertyInfo.Name} cannot be used as SQL column because it is on a struct; use an explicit backing field as SQL column instead");
			case PropertyInfo propertyInfo when propertyInfo.GetIndexParameters().Length > 0:
				throw new InvalidOperationException($"The property {declaringType.FullName}.{propertyInfo.Name} cannot be used as SQL column because it is indexed");
			case PropertyInfo propertyInfo when propertyInfo.GetGetMethod(true) == null:
				throw new InvalidOperationException($"The property {declaringType.FullName}.{propertyInfo.Name} cannot be used as SQL column because it has no getter");
			case PropertyInfo propertyInfo when propertyInfo.GetSetMethod(true) == null:
				throw new InvalidOperationException($"The property {declaringType.FullName}.{propertyInfo.Name} cannot be used as SQL column because it has no setter");
			case PropertyInfo _:
				break;
			default:
				throw new ArgumentException("Only fields and properties are supported", nameof(memberInfo));
			}
		}

		public SqlDbType DbType => dbType;

		public bool IsNativeType => isNativeType;

		public IDictionary<string, SqlColumnInfo> Columns => columns;

		public ReadOnlyCollection<IMemberConverter> Converters => converters;

		public bool HasNestedSerializers => hasNestedSerializers;

		public int MemberCount => members.Length;

		public object GetMember(object instance, int index) {
			Debug.Assert((instance != null) && (index >= 0) && (methods.GetMember != null));
			return methods.GetMember(instance, index);
		}

		public void PopulateInstanceMembers(object result, object[] buffer) {
			Debug.Assert((result != null) && (buffer != null) && (methods.PopulateMembers != null));
			methods.PopulateMembers(result, buffer);
		}
	}
}

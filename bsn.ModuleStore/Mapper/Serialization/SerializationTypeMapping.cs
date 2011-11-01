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
				KeyValuePair<SqlDbType, bool> dbType;
				return dbTypeMapping.TryGetValue(type, out dbType) && dbType.Value;
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
				KeyValuePair<SqlDbType, bool> mapping;
				lock (dbTypeMapping) {
					if (dbTypeMapping.TryGetValue(type, out mapping)) {
						return mapping.Key;
					}
					SqlDbType result;
					if (type.IsDefined(typeof(SqlUserDefinedTypeAttribute), false)) {
						result = SqlDbType.Udt;
					} else {
						if (type.IsXmlType()) {
							result = SqlDbType.Xml;
						} else {
							Type dummyType;
							if (type.TryGetIEnumerableElementType(out dummyType)) {
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
				throw new ArgumentNullException("type");
			}
			// required to enable recursive resolution of mappings
			typeMappingProvider.RegisterMapping(type, this);
			List<IMemberConverter> memberConverters = new List<IMemberConverter>();
			List<MemberInfo> memberInfos = new List<MemberInfo>();
			isNativeType = CheckNativeType(type);
			dbType = GetTypeMapping(type);
			if (!(type.IsPrimitive || type.IsInterface || (typeof(string) == type))) {
				bool hasIdentity = false;
				foreach (MemberInfo member in type.GetAllFieldsAndProperties()) {
					SqlColumnAttribute columnAttribute = SqlColumnAttributeBase.Get<SqlColumnAttribute>(member, false);
					Type memberType = member.GetMemberType();
					if (columnAttribute != null) {
						AssertValidMember(member);
						bool isIdentity = (!hasIdentity) && (hasIdentity |= columnAttribute.Identity);
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
			FieldInfo fieldInfo = memberInfo as FieldInfo;
			if (fieldInfo != null) {
				if (fieldInfo.IsInitOnly) {
					throw new InvalidOperationException(String.Format("The field {0}.{1} cannot be used as SQL column because it is readonly", fieldInfo.DeclaringType.FullName, fieldInfo.Name));
				}
			} else {
				PropertyInfo propertyInfo = memberInfo as PropertyInfo;
				if (propertyInfo != null) {
					if (propertyInfo.GetIndexParameters().Length > 0) {
						throw new InvalidOperationException(String.Format("The property {0}.{1} cannot be used as SQL column because it is indexed", propertyInfo.DeclaringType.FullName, propertyInfo.Name));
					}
					if (propertyInfo.GetGetMethod(true) == null) {
						throw new InvalidOperationException(String.Format("The property {0}.{1} cannot be used as SQL column because it has no getter", propertyInfo.DeclaringType.FullName, propertyInfo.Name));
					}
					if (propertyInfo.GetSetMethod(true) == null) {
						throw new InvalidOperationException(String.Format("The property {0}.{1} cannot be used as SQL column because it has no setter", propertyInfo.DeclaringType.FullName, propertyInfo.Name));
					}
				} else {
					throw new ArgumentException("Only fields and properties are supported", "memberInfo");
				}
			}
		}

		public SqlDbType DbType {
			get {
				return dbType;
			}
		}

		public bool IsNativeType {
			get {
				return isNativeType;
			}
		}

		public IDictionary<string, SqlColumnInfo> Columns {
			get {
				return columns;
			}
		}

		public ReadOnlyCollection<IMemberConverter> Converters {
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
			Debug.Assert((instance != null) && (index >= 0) && (methods.GetMember != null));
			return methods.GetMember(instance, index);
		}

		public void PopulateInstanceMembers(object result, object[] buffer) {
			Debug.Assert((result != null) && (buffer != null) && (methods.PopulateMembers != null));
			methods.PopulateMembers(result, buffer);
		}
	}
}

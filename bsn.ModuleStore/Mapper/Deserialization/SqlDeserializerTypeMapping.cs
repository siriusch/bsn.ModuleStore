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
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace bsn.ModuleStore.Mapper.Deserialization {
	internal class SqlDeserializerTypeMapping {
		private static readonly Dictionary<Type, SqlDeserializerTypeMapping> mappings = new Dictionary<Type, SqlDeserializerTypeMapping>();

		public static SqlDeserializerTypeMapping Get(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			SqlDeserializerTypeMapping result;
			lock (mappings) {
				if (!mappings.TryGetValue(type, out result)) {
					result = new SqlDeserializerTypeMapping(type);
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

		private readonly ReadOnlyCollection<MemberConverter> converters;
		private readonly MemberInfo[] members;

		private SqlDeserializerTypeMapping(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			List<MemberConverter> memberConverters = new List<MemberConverter>();
			List<MemberInfo> memberInfos = new List<MemberInfo>();
			if (!(type.IsPrimitive || type.IsInterface || (typeof(string) == type))) {
				bool hasIdentity = false;
				foreach (FieldInfo field in GetAllFields(type)) {
					SqlColumnAttribute columnAttribute = SqlColumnAttribute.GetColumnAttribute(field, false);
					if (columnAttribute != null) {
						bool isIdentity = (!hasIdentity) && (hasIdentity |= columnAttribute.Identity);
						if (columnAttribute.GetCachedByIdentity) {
							memberConverters.Add(new CachedMemberConverter(field.FieldType, isIdentity, columnAttribute.Name, memberInfos.Count, columnAttribute.DateTimeKind));
						} else {
							memberConverters.Add(MemberConverter.Get(field.FieldType, isIdentity, columnAttribute.Name, memberInfos.Count, columnAttribute.DateTimeKind));
						}
						memberInfos.Add(field);
					} else if (field.IsDefined(typeof(SqlDeserializeAttribute), true)) {
						memberConverters.Add(new NestedMemberConverter(field.FieldType, memberInfos.Count));
						memberInfos.Add(field);
					}
				}
			}
			members = memberInfos.ToArray();
			converters = Array.AsReadOnly(memberConverters.ToArray());
		}

		public ReadOnlyCollection<MemberConverter> Converters {
			get {
				return converters;
			}
		}

		public int MemberCount {
			get {
				return members.Length;
			}
		}

		public void PopulateInstanceMembers(object result, object[] buffer) {
			FormatterServices.PopulateObjectMembers(result, members, buffer);
		}
	}
}

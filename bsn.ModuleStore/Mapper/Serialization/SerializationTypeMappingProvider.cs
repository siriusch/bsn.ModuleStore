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

namespace bsn.ModuleStore.Mapper.Serialization {
	public class SerializationTypeMappingProvider: ISerializationTypeMappingProvider {
		private static readonly Dictionary<Type, SerializationTypeMapping> mappings = new Dictionary<Type, SerializationTypeMapping>();

		public ISerializationTypeMapping GetMapping(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			SerializationTypeMapping result;
			lock (mappings) {
				if (!mappings.TryGetValue(type, out result)) {
					result = new SerializationTypeMapping(type, this);
					mappings.Add(type, result);
				}
			}
			return result;
		}

		/// <summary>
		/// Get a single <see cref="SqlColumnAttribute"/> instance.
		/// </summary>
		/// <param name="member">The <see cref="MemberInfo"/> to query for the <see cref="SqlColumnAttribute"/> attribute.</param>
		/// <param name="autoCreate">If true, a <see cref="SqlColumnAttribute"/> is inferred from the MemberInfo when no attribute is found. Otherwise, null is returned in this situation.</param>
		/// <returns>The <see cref="SqlColumnAttribute"/> for the member.</returns>
		public virtual SqlColumnAttribute GetSqlColumnAttribute(MemberInfo member, bool autoCreate) {
			if (member == null) {
				throw new ArgumentNullException("member");
			}
			SqlColumnAttribute[] columnAttributes = (SqlColumnAttribute[])member.GetCustomAttributes(typeof(SqlColumnAttribute), true);
			if (columnAttributes.Length > 0) {
				SqlColumnAttribute result = columnAttributes[0];
				if (string.IsNullOrEmpty(result.Name)) {
					result = result.CloneWithName(member.Name);
				}
				return result;
			}
			if (autoCreate) {
				return new SqlColumnAttribute(member.Name);
			}
			return null;
		}
	}
}
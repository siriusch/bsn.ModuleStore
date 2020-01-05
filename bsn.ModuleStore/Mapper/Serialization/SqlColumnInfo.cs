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
using System.Data;
using System.Reflection;

namespace bsn.ModuleStore.Mapper.Serialization {
	public class SqlColumnInfo {
		private readonly IMemberConverter converter;
		private readonly MemberInfo memberInfo;
		private readonly string name;
		private readonly ISerializationTypeMapping typeMapping;

		public SqlColumnInfo(ISerializationTypeMapping typeMapping, string columnName, IMemberConverter converter) {
			if (string.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException(nameof(columnName));
			}
			if (converter == null) {
				throw new ArgumentNullException(nameof(converter));
			}
			if (typeMapping == null) {
				throw new ArgumentNullException(nameof(typeMapping));
			}
			name = columnName;
			this.converter = converter;
			this.typeMapping = typeMapping;
		}

		public SqlColumnInfo(MemberInfo memberInfo, string columnName, IMemberConverter converter, ISerializationTypeMapping typeMapping): this(typeMapping, columnName, converter) {
			if (memberInfo == null) {
				throw new ArgumentNullException(nameof(memberInfo));
			}
			this.memberInfo = memberInfo;
		}

		public IMemberConverter Converter => converter;

		public SqlDbType DbType => typeMapping.DbType;

		public MemberInfo MemberInfo => memberInfo;

		public string Name => name;
	}
}

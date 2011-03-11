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
using System.Data;
using System.Reflection;

namespace bsn.ModuleStore.Mapper.Serialization {
	internal class SqlColumnInfo {
		private readonly Type clrType;
		private readonly MemberConverter converter;
		private readonly FieldInfo fieldInfo;
		private readonly string name;
		private readonly string userDefinedTypeName;

		public SqlColumnInfo(Type fieldType, string columnName, MemberConverter converter) {
			if (string.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException("columnName");
			}
			if (converter == null) {
				throw new ArgumentNullException("converter");
			}
			name = columnName;
			clrType = Nullable.GetUnderlyingType(fieldType) ?? fieldType;
			userDefinedTypeName = null;
#warning SqlSerializationTypeMapping.GetClrUserDefinedTypeName(fieldInfo.DeclaringType, columnAttribute);
			this.converter = converter;
		}

		public SqlColumnInfo(FieldInfo fieldInfo, string columnName, MemberConverter converter): this(fieldInfo.FieldType, columnName, converter) {
			this.fieldInfo = fieldInfo;
		}

		public MemberConverter Converter {
			get {
				return converter;
			}
		}

		public SqlDbType DbType {
			get {
				return SqlSerializationTypeMapping.GetTypeMapping(clrType);
			}
		}

		public FieldInfo FieldInfo {
			get {
				return fieldInfo;
			}
		}

		public string Name {
			get {
				return name;
			}
		}

		public string UserDefinedTypeName {
			get {
				return userDefinedTypeName;
			}
		}
	}
}

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
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace bsn.ModuleStore.Mapper.Serialization {
	public class MemberConverter: IMemberConverter {
		public static IMemberConverter Get(Type type, bool isIdentity, string columnName, int memberIndex, DateTimeKind dateTimeKind) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			type = Nullable.GetUnderlyingType(type) ?? type;
			if (type.IsEnum) {
				return new EnumMemberConverter(type, isIdentity, columnName, memberIndex);
			}
			if (typeof(XmlReader).Equals(type)) {
				return new XmlReaderMemberConverter(type, isIdentity, columnName, memberIndex);
			}
			if (typeof(XDocument).Equals(type)) {
				return new XDocumentMemberConverter(type, isIdentity, columnName, memberIndex);
			}
			if (typeof(XElement).Equals(type)) {
				return new XElementMemberConverter(type, isIdentity, columnName, memberIndex);
			}
			if (typeof(XmlElement).Equals(type)) {
				return new XmlElementMemberConverter(type, isIdentity, columnName, memberIndex);
			}
			if (typeof(XmlDocument).Equals(type)) {
				return new XmlDocumentMemberConverter(type, isIdentity, columnName, memberIndex);
			}
			if (typeof(XPathDocument).Equals(type)) {
				return new XPathDocumentMemberConverter(type, isIdentity, columnName, memberIndex);
			}
			if (typeof(DateTime).Equals(type)) {
				return new DateTimeMemberConverter(type, isIdentity, columnName, memberIndex, dateTimeKind);
			}
			if (typeof(DateTimeOffset).Equals(type)) {
				return new DateTimeOffsetMemberConverter(type, isIdentity, columnName, memberIndex, dateTimeKind);
			}
			if (typeof(IConvertible).IsAssignableFrom(type)) {
				return new ConvertibleMemberConverter(type, isIdentity, columnName, memberIndex);
			}
			return new MemberConverter(type, isIdentity, columnName, memberIndex);
		}

		private readonly string columnName;
		private readonly bool isIdentity;
		private readonly int memberIndex;
		private readonly Type type;

		internal MemberConverter(Type type, bool isIdentity, string columnName, int memberIndex) {
			this.type = type;
			this.isIdentity = isIdentity;
			this.columnName = columnName;
			this.memberIndex = memberIndex;
		}

		public string ColumnName {
			get {
				return columnName;
			}
		}

		public virtual Type DbClrType {
			get {
				return type;
			}
		}

		public bool IsIdentity {
			get {
				return isIdentity;
			}
		}

		public int MemberIndex {
			get {
				return memberIndex;
			}
		}

		public Type Type {
			get {
				return type;
			}
		}

		public virtual object ProcessFromDb(IDeserializerContext context, int column) {
			object result = context.DataReader.GetValue(column);
			if (result == DBNull.Value) {
				return null;
			}
			return result;
		}

		public virtual object ProcessToDb(object value) {
			if (value == null) {
				return DBNull.Value;
			}
			return value;
		}
	}
}

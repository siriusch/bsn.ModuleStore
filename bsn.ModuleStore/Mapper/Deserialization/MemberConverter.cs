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

namespace bsn.ModuleStore.Mapper.Deserialization {
	internal class MemberConverter {
		public static MemberConverter Get(Type type, int memberIndex, DateTimeKind dateTimeKind) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			if (type.IsEnum) {
				return new EnumMemberConverter(type, memberIndex);
			}
			if (typeof(XmlReader).Equals(type)) {
				return new XmlReaderMemberConverter(type, memberIndex);
			}
			if (typeof(XDocument).Equals(type)) {
				return new XDocumentMemberConverter(type, memberIndex);
			}
			if (typeof(XElement).Equals(type)) {
				return new XElementMemberConverter(type, memberIndex);
			}
			if (typeof(XmlElement).Equals(type)) {
				return new XmlElementMemberConverter(type, memberIndex);
			}
			if (typeof(XmlDocument).Equals(type)) {
				return new XmlDocumentMemberConverter(type, memberIndex);
			}
			if (typeof(XPathDocument).Equals(type)) {
				return new XPathDocumentMemberConverter(type, memberIndex);
			}
			if (typeof(DateTime).Equals(type)) {
				return new DateTimeMemberConverter(type, memberIndex, dateTimeKind);
			}
			if (typeof(DateTimeOffset).Equals(type)) {
				return new DateTimeOffsetMemberConverter(type, memberIndex, dateTimeKind);
			}
			if (typeof(IConvertible).IsAssignableFrom(type)) {
				return new ConvertibleMemberConverter(type, memberIndex);
			}
			return new MemberConverter(type, memberIndex);
		}

		private readonly int memberMemberIndex;
		private readonly Type type;

		internal MemberConverter(Type type, int memberMemberIndex) {
			this.type = type;
			this.memberMemberIndex = memberMemberIndex;
		}

		public int MemberIndex {
			get {
				return memberMemberIndex;
			}
		}

		public Type Type {
			get {
				return type;
			}
		}

		public virtual object Process(SqlDeserializer.DeserializerContext context, int column) {
			object result = context.DataReader.GetValue(column);
			if (result == DBNull.Value) {
				return null;
			}
			return result;
		}
	}
}

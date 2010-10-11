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
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace bsn.ModuleStore.Mapper {
	internal class MemberConverter {
		private class ConvertibleMemberConverter: MemberConverter {
			public ConvertibleMemberConverter(Type type, int memberIndex): base(type, memberIndex) {}

			public override object Process(SqlDeserializer.DeserializerContext context, int column) {
				object result = base.Process(context, column);
				if ((result != null) && (!type.IsAssignableFrom(result.GetType()))) {
					result = Convert.ChangeType(result, Type);
				}
				return result;
			}
		}

		private class DateTimeMemberConverter: MemberConverter {
			private readonly DateTimeKind dateTimeKind;

			public DateTimeMemberConverter(Type type, int memberIndex, DateTimeKind dateTimeKind): base(type, memberIndex) {
				this.dateTimeKind = dateTimeKind;
			}

			public override object Process(SqlDeserializer.DeserializerContext context, int column) {
				object result = base.Process(context, column);
				if (result != null) {
					result = DateTime.SpecifyKind(Convert.ToDateTime(result), dateTimeKind);
				}
				return result;
			}
		}

		private class DateTimeOffsetMemberConverter: MemberConverter {
			private readonly DateTimeKind dateTimeKind;

			public DateTimeOffsetMemberConverter(Type type, int memberIndex, DateTimeKind dateTimeKind): base(type, memberIndex) {
				this.dateTimeKind = dateTimeKind;
			}

			public override object Process(SqlDeserializer.DeserializerContext context, int column) {
				object result = base.Process(context, column);
				if ((result != null) && (!(result is DateTimeOffset))) {
					result = new DateTimeOffset(DateTime.SpecifyKind(Convert.ToDateTime(result), dateTimeKind));
				}
				return result;
			}
		}

		private class XDocumentMemberConverter: XmlReaderMemberConverterBase {
			public XDocumentMemberConverter(Type type, int memberIndex): base(type, memberIndex) {}

			protected override object GetXmlObject(SqlDeserializer.DeserializerContext context, XmlReader reader) {
				return XDocument.Load(reader);
			}
		}

		private class XElementMemberConverter: XmlReaderMemberConverterBase {
			public XElementMemberConverter(Type type, int memberIndex): base(type, memberIndex) {}

			protected override object GetXmlObject(SqlDeserializer.DeserializerContext context, XmlReader reader) {
				return XElement.Load(reader);
			}
		}

		private class XPathDocumentMemberConverter: XmlReaderMemberConverterBase {
			public XPathDocumentMemberConverter(Type type, int memberIndex): base(type, memberIndex) {}

			protected override object GetXmlObject(SqlDeserializer.DeserializerContext context, XmlReader reader) {
				return new XPathDocument(reader);
			}
		}

		private class XmlDocumentMemberConverter: XmlReaderMemberConverterBase {
			public XmlDocumentMemberConverter(Type type, int memberIndex): base(type, memberIndex) {}

			protected override object GetXmlObject(SqlDeserializer.DeserializerContext context, XmlReader reader) {
				XmlDocument doc = new XmlDocument(context.NameTable);
				doc.Load(reader);
				return doc;
			}
		}

		private class XmlElementMemberConverter: XmlReaderMemberConverterBase {
			public XmlElementMemberConverter(Type type, int memberIndex): base(type, memberIndex) {}

			protected override object GetXmlObject(SqlDeserializer.DeserializerContext context, XmlReader reader) {
				context.XmlDocument.Load(reader);
				XmlElement result = context.XmlDocument.DocumentElement;
				context.XmlDocument.RemoveAll();
				return result;
			}
		}

		private class XmlReaderMemberConverter: MemberConverter {
			public XmlReaderMemberConverter(Type type, int memberIndex): base(type, memberIndex) {}

			public override object Process(SqlDeserializer.DeserializerContext context, int column) {
				if (context.DataReader.IsDBNull(column)) {
					return null;
				}
				XmlReader xmlReader;
				SqlDataReader sqlDataReader = context.DataReader as SqlDataReader;
				if (sqlDataReader != null) {
					xmlReader = sqlDataReader.GetSqlXml(column).CreateReader();
				} else {
					xmlReader = new XmlTextReader(new StringReader(context.DataReader.GetString(column)));
				}
				return ProcessXmlReader(context, xmlReader);
			}

			protected virtual object ProcessXmlReader(SqlDeserializer.DeserializerContext context, XmlReader xmlReader) {
				return xmlReader;
			}
		}

		private abstract class XmlReaderMemberConverterBase: XmlReaderMemberConverter {
			protected XmlReaderMemberConverterBase(Type type, int memberIndex): base(type, memberIndex) {}

			protected abstract object GetXmlObject(SqlDeserializer.DeserializerContext context, XmlReader reader);

			protected override sealed object ProcessXmlReader(SqlDeserializer.DeserializerContext context, XmlReader xmlReader) {
				using (xmlReader) {
					return GetXmlObject(context, xmlReader);
				}
			}
		}

		public static MemberConverter Get(Type type, int memberIndex, DateTimeKind dateTimeKind) {
			if (type == null) {
				throw new ArgumentNullException("type");
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

		private readonly int memberIndex;
		private readonly Type type;

		internal MemberConverter(Type type, int memberIndex) {
			this.type = type;
			this.memberIndex = memberIndex;
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

		public virtual object Process(SqlDeserializer.DeserializerContext context, int column) {
			object result = context.DataReader.GetValue(column);
			if (ReferenceEquals(result, DBNull.Value)) {
				return null;
			}
			return result;
		}
	}
}

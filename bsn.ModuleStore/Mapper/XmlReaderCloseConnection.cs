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
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Schema;

namespace bsn.ModuleStore.Mapper {
	internal sealed class XmlReaderCloseConnection: XmlReader {
		private readonly SqlConnection connection;
		private readonly XmlReader reader;

		public XmlReaderCloseConnection(XmlReader reader, SqlConnection connection) {
			if (connection == null) {
				throw new ArgumentNullException("connection");
			}
			if (reader == null) {
				throw new ArgumentNullException("reader");
			}
			this.connection = connection;
			this.reader = reader;
		}

		public override string this[string name, string namespaceURI] {
			get {
				return reader[name, namespaceURI];
			}
		}

		public override string this[string name] {
			get {
				return reader[name];
			}
		}

		public override string this[int i] {
			get {
				return reader[i];
			}
		}

		public override int AttributeCount {
			get {
				return reader.AttributeCount;
			}
		}

		public override string BaseURI {
			get {
				return reader.BaseURI;
			}
		}

		public override bool CanReadBinaryContent {
			get {
				return reader.CanReadBinaryContent;
			}
		}

		public override bool CanReadValueChunk {
			get {
				return reader.CanReadValueChunk;
			}
		}

		public override bool CanResolveEntity {
			get {
				return reader.CanResolveEntity;
			}
		}

		public override int Depth {
			get {
				return reader.Depth;
			}
		}

		public override bool EOF {
			get {
				return reader.EOF;
			}
		}

		public override bool HasAttributes {
			get {
				return reader.HasAttributes;
			}
		}

		public override bool HasValue {
			get {
				return reader.HasValue;
			}
		}

		public override bool IsDefault {
			get {
				return reader.IsDefault;
			}
		}

		public override bool IsEmptyElement {
			get {
				return reader.IsEmptyElement;
			}
		}

		public override string LocalName {
			get {
				return reader.LocalName;
			}
		}

		public override string Name {
			get {
				return reader.Name;
			}
		}

		public override XmlNameTable NameTable {
			get {
				return reader.NameTable;
			}
		}

		public override string NamespaceURI {
			get {
				return reader.NamespaceURI;
			}
		}

		public override XmlNodeType NodeType {
			get {
				return reader.NodeType;
			}
		}

		public override string Prefix {
			get {
				return reader.Prefix;
			}
		}

		public override char QuoteChar {
			get {
				return reader.QuoteChar;
			}
		}

		public override ReadState ReadState {
			get {
				return reader.ReadState;
			}
		}

		public XmlReader Reader {
			get {
				return reader;
			}
		}

		public override IXmlSchemaInfo SchemaInfo {
			get {
				return reader.SchemaInfo;
			}
		}

		public override XmlReaderSettings Settings {
			get {
				return reader.Settings;
			}
		}

		public override string Value {
			get {
				return reader.Value;
			}
		}

		public override Type ValueType {
			get {
				return reader.ValueType;
			}
		}

		public override string XmlLang {
			get {
				return reader.XmlLang;
			}
		}

		public override XmlSpace XmlSpace {
			get {
				return reader.XmlSpace;
			}
		}

		public override void Close() {
			Dispose(true);
		}

		public override string GetAttribute(int i) {
			return reader.GetAttribute(i);
		}

		public override string GetAttribute(string name) {
			return reader.GetAttribute(name);
		}

		public override string GetAttribute(string name, string namespaceURI) {
			return reader.GetAttribute(name, namespaceURI);
		}

		public override bool IsStartElement(string name) {
			return reader.IsStartElement(name);
		}

		public override bool IsStartElement(string localname, string ns) {
			return reader.IsStartElement(localname, ns);
		}

		public override bool IsStartElement() {
			return reader.IsStartElement();
		}

		public override string LookupNamespace(string prefix) {
			return reader.LookupNamespace(prefix);
		}

		public override bool MoveToAttribute(string name) {
			return reader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string name, string ns) {
			return reader.MoveToAttribute(name, ns);
		}

		public override void MoveToAttribute(int i) {
			reader.MoveToAttribute(i);
		}

		public override XmlNodeType MoveToContent() {
			return reader.MoveToContent();
		}

		public override bool MoveToElement() {
			return reader.MoveToElement();
		}

		public override bool MoveToFirstAttribute() {
			return reader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute() {
			return reader.MoveToNextAttribute();
		}

		public override bool Read() {
			return reader.Read();
		}

		public override bool ReadAttributeValue() {
			return reader.ReadAttributeValue();
		}

		public override object ReadContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver) {
			return reader.ReadContentAs(returnType, namespaceResolver);
		}

		public override int ReadContentAsBase64(byte[] buffer, int index, int count) {
			return reader.ReadContentAsBase64(buffer, index, count);
		}

		public override int ReadContentAsBinHex(byte[] buffer, int index, int count) {
			return reader.ReadContentAsBinHex(buffer, index, count);
		}

		public override bool ReadContentAsBoolean() {
			return reader.ReadContentAsBoolean();
		}

		public override DateTime ReadContentAsDateTime() {
			return reader.ReadContentAsDateTime();
		}

		public override decimal ReadContentAsDecimal() {
			return reader.ReadContentAsDecimal();
		}

		public override double ReadContentAsDouble() {
			return reader.ReadContentAsDouble();
		}

		public override float ReadContentAsFloat() {
			return reader.ReadContentAsFloat();
		}

		public override int ReadContentAsInt() {
			return reader.ReadContentAsInt();
		}

		public override long ReadContentAsLong() {
			return reader.ReadContentAsLong();
		}

		public override object ReadContentAsObject() {
			return reader.ReadContentAsObject();
		}

		public override string ReadContentAsString() {
			return reader.ReadContentAsString();
		}

		public override object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver) {
			return reader.ReadElementContentAs(returnType, namespaceResolver);
		}

		public override object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver, string localName, string namespaceURI) {
			return reader.ReadElementContentAs(returnType, namespaceResolver, localName, namespaceURI);
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int index, int count) {
			return reader.ReadElementContentAsBase64(buffer, index, count);
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count) {
			return reader.ReadElementContentAsBinHex(buffer, index, count);
		}

		public override bool ReadElementContentAsBoolean() {
			return reader.ReadElementContentAsBoolean();
		}

		public override bool ReadElementContentAsBoolean(string localName, string namespaceURI) {
			return reader.ReadElementContentAsBoolean(localName, namespaceURI);
		}

		public override DateTime ReadElementContentAsDateTime() {
			return reader.ReadElementContentAsDateTime();
		}

		public override DateTime ReadElementContentAsDateTime(string localName, string namespaceURI) {
			return reader.ReadElementContentAsDateTime(localName, namespaceURI);
		}

		public override decimal ReadElementContentAsDecimal(string localName, string namespaceURI) {
			return reader.ReadElementContentAsDecimal(localName, namespaceURI);
		}

		public override decimal ReadElementContentAsDecimal() {
			return reader.ReadElementContentAsDecimal();
		}

		public override double ReadElementContentAsDouble() {
			return reader.ReadElementContentAsDouble();
		}

		public override double ReadElementContentAsDouble(string localName, string namespaceURI) {
			return reader.ReadElementContentAsDouble(localName, namespaceURI);
		}

		public override float ReadElementContentAsFloat(string localName, string namespaceURI) {
			return reader.ReadElementContentAsFloat(localName, namespaceURI);
		}

		public override float ReadElementContentAsFloat() {
			return reader.ReadElementContentAsFloat();
		}

		public override int ReadElementContentAsInt() {
			return reader.ReadElementContentAsInt();
		}

		public override int ReadElementContentAsInt(string localName, string namespaceURI) {
			return reader.ReadElementContentAsInt(localName, namespaceURI);
		}

		public override long ReadElementContentAsLong(string localName, string namespaceURI) {
			return reader.ReadElementContentAsLong(localName, namespaceURI);
		}

		public override long ReadElementContentAsLong() {
			return reader.ReadElementContentAsLong();
		}

		public override object ReadElementContentAsObject(string localName, string namespaceURI) {
			return reader.ReadElementContentAsObject(localName, namespaceURI);
		}

		public override object ReadElementContentAsObject() {
			return reader.ReadElementContentAsObject();
		}

		public override string ReadElementContentAsString() {
			return reader.ReadElementContentAsString();
		}

		public override string ReadElementContentAsString(string localName, string namespaceURI) {
			return reader.ReadElementContentAsString(localName, namespaceURI);
		}

		public override string ReadElementString() {
			return reader.ReadElementString();
		}

		public override string ReadElementString(string name) {
			return reader.ReadElementString(name);
		}

		public override string ReadElementString(string localname, string ns) {
			return reader.ReadElementString(localname, ns);
		}

		public override void ReadEndElement() {
			reader.ReadEndElement();
		}

		public override string ReadInnerXml() {
			return reader.ReadInnerXml();
		}

		public override string ReadOuterXml() {
			return reader.ReadOuterXml();
		}

		public override void ReadStartElement() {
			reader.ReadStartElement();
		}

		public override void ReadStartElement(string name) {
			reader.ReadStartElement(name);
		}

		public override void ReadStartElement(string localname, string ns) {
			reader.ReadStartElement(localname, ns);
		}

		public override string ReadString() {
			return reader.ReadString();
		}

		public override XmlReader ReadSubtree() {
			return reader.ReadSubtree();
		}

		public override bool ReadToDescendant(string name) {
			return reader.ReadToDescendant(name);
		}

		public override bool ReadToDescendant(string localName, string namespaceURI) {
			return reader.ReadToDescendant(localName, namespaceURI);
		}

		public override bool ReadToFollowing(string name) {
			return reader.ReadToFollowing(name);
		}

		public override bool ReadToFollowing(string localName, string namespaceURI) {
			return reader.ReadToFollowing(localName, namespaceURI);
		}

		public override bool ReadToNextSibling(string localName, string namespaceURI) {
			return reader.ReadToNextSibling(localName, namespaceURI);
		}

		public override bool ReadToNextSibling(string name) {
			return reader.ReadToNextSibling(name);
		}

		public override int ReadValueChunk(char[] buffer, int index, int count) {
			return reader.ReadValueChunk(buffer, index, count);
		}

		public override void ResolveEntity() {
			reader.ResolveEntity();
		}

		public override void Skip() {
			reader.Skip();
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				try {
					((IDisposable)reader).Dispose();
				} finally {
					connection.Dispose();
				}
			}
		}
	}
}

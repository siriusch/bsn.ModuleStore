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
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace bsn.ModuleStore.Mapper.Serialization {
	internal class StructuredParameterReader: DbDataReader {
		private readonly IEnumerator enumerator;
		private readonly StructuredParameterSchema schema;
		private object[] data;
		private bool isClosed;
		private int rowCount;

		public StructuredParameterReader(StructuredParameterSchema schema, IEnumerable values) {
			if (schema == null) {
				throw new ArgumentNullException("schema");
			}
			this.schema = schema;
			enumerator = values.GetEnumerator();
			if (!enumerator.MoveNext()) {
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null) {
					disposable.Dispose();
				}
				enumerator = null;
			}
		}

		public override object this[int ordinal] {
			get {
				return GetValueInternal(ordinal);
			}
		}

		public override object this[string name] {
			get {
				return GetValueInternal(GetOrdinal(name));
			}
		}

		public override int Depth {
			get {
				return 0;
			}
		}

		public override int FieldCount {
			get {
				return schema.ColumnCount;
			}
		}

		public override bool HasRows {
			get {
				return (enumerator != null);
			}
		}

		public override bool IsClosed {
			get {
				return isClosed;
			}
		}

		public override int RecordsAffected {
			get {
				return rowCount;
			}
		}

		public override void Close() {
			if (!isClosed) {
				Dispose(true);
			}
		}

		public override bool GetBoolean(int i) {
			return (bool)GetValueInternal(i);
		}

		public override byte GetByte(int i) {
			return (byte)GetValueInternal(i);
		}

		public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferOffset, int length) {
			if (length > 0) {
				object value = GetValueInternal(i);
				XmlReader reader = value as XmlReader;
				if (reader != null) {
					CopyXmlReaderContent(reader, XmlWriter.Create((Stream)(value = new MemoryStream()), CreateXmlWriterSettings()));
					SetValueInternal(i, value);
				}
				byte[] bytes = value as byte[];
				if (bytes != null) {
					if (fieldOffset > bytes.Length) {
						return 0;
					}
					length = Math.Min(bytes.Length-(int)fieldOffset, length);
					Array.Copy(bytes, fieldOffset, buffer, bufferOffset, length);
					return length;
				}
				Stream stream = value as Stream;
				if (stream != null) {
					if (stream.Position != fieldOffset) {
						stream.Seek(fieldOffset, SeekOrigin.Begin);
					}
					return stream.Read(buffer, bufferOffset, length);
				}
			}
			return 0;
		}

		public override char GetChar(int i) {
			return (char)GetValueInternal(i);
		}

		public override long GetChars(int i, long fieldOffset, char[] buffer, int bufferOffset, int length) {
			if (length > 0) {
				object value = GetValueInternal(i);
				if (value is char) {
					if (fieldOffset > 0) {
						return 0;
					}
					buffer[bufferOffset] = (char)value;
					return 1;
				}
				XmlReader reader = value as XmlReader;
				if (reader != null) {
					using (StringWriter output = new StringWriter()) {
						CopyXmlReaderContent(reader, XmlWriter.Create(output, CreateXmlWriterSettings()));
						value = output.ToString();
						SetValueInternal(i, value);
					}
				}
				string str = value as string;
				if (str != null) {
					if (fieldOffset > str.Length) {
						return 0;
					}
					length = Math.Min(str.Length-(int)fieldOffset, length);
					for (i = 0; i < length; i++) {
						buffer[bufferOffset+i] = str[i];
					}
					return length;
				}
				char[] chars = value as char[];
				if (chars != null) {
					if (fieldOffset > chars.Length) {
						return 0;
					}
					length = Math.Min(chars.Length-(int)fieldOffset, length);
					Array.Copy(chars, fieldOffset, buffer, bufferOffset, length);
					return length;
				}
			}
			return 0;
		}

		public override string GetDataTypeName(int i) {
			return schema.GetColumnTypeName(i);
		}

		public override DateTime GetDateTime(int i) {
			return (DateTime)GetValueInternal(i);
		}

		public override decimal GetDecimal(int i) {
			return (decimal)GetValueInternal(i);
		}

		public override double GetDouble(int i) {
			return (double)GetValueInternal(i);
		}

		public override IEnumerator GetEnumerator() {
			throw new NotSupportedException("Iteration over the reader is not supported");
		}

		public override Type GetFieldType(int i) {
			return schema.GetColumnDataType(i);
		}

		public override float GetFloat(int i) {
			return (float)GetValueInternal(i);
		}

		public override Guid GetGuid(int i) {
			return (Guid)GetValueInternal(i);
		}

		public override short GetInt16(int i) {
			return (short)GetValueInternal(i);
		}

		public override int GetInt32(int i) {
			return (int)GetValueInternal(i);
		}

		public override long GetInt64(int i) {
			return (long)GetValueInternal(i);
		}

		public override string GetName(int i) {
			return schema.GetColumnName(i);
		}

		public override int GetOrdinal(string name) {
			for (int i = 0; i < schema.ColumnCount; i++) {
				if (string.Equals(schema.GetColumnName(i), name, StringComparison.OrdinalIgnoreCase)) {
					return i;
				}
			}
			throw new IndexOutOfRangeException(string.Format("Could not find the column '{0}'", name));
		}

		public override DataTable GetSchemaTable() {
			return schema;
		}

		public override string GetString(int i) {
			object value = GetValueInternal(i);
			return (value is string) ? (string)value : value.ToString();
		}

		public override object GetValue(int ordinal) {
			object value = GetValueInternal(ordinal);
			XmlReader reader = value as XmlReader;
			if (reader != null) {
				using (reader) {
					value = new SqlXml(reader);
				}
				SetValueInternal(ordinal, value);
			}
			return value;
		}

		public override int GetValues(object[] values) {
			if (values == null) {
				throw new ArgumentNullException("values");
			}
			for (int i = 0; i < schema.ColumnCount; i++) {
				values[i] = GetValueInternal(i);
			}
			return schema.ColumnCount;
		}

		public override bool IsDBNull(int i) {
			int fieldIndex;
			if (schema.TryGetFieldIndexOfColumn(i, out fieldIndex)) {
				return data[fieldIndex] == DBNull.Value;
			}
			return true;
		}

		public override bool NextResult() {
			return false;
		}

		public override bool Read() {
			// we already performed the first MoveNext() call so we have to return true on the first run (RowCount == 0) and increment rowcount so that the second run is a real MoveNext() call
			if (HasRows) {
				if ((rowCount == 0) || enumerator.MoveNext()) {
					LoadData(enumerator.Current);
					rowCount++;
					return true;
				}
			}
			LoadData(null);
			return false;
		}

		protected override void Dispose(bool disposing) {
			if (disposing && (!isClosed)) {
				isClosed = true;
				IDisposable disposableEnumerator = enumerator as IDisposable;
				if (disposableEnumerator != null) {
					disposableEnumerator.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private void CopyXmlReaderContent(XmlReader reader, XmlWriter writer) {
			using (writer) {
				reader.MoveToContent();
				Debug.Assert(reader.ReadState == ReadState.Interactive);
				while (!reader.EOF) {
					writer.WriteNode(reader, false);
				}
			}
			reader.Close();
		}

		private XmlWriterSettings CreateXmlWriterSettings() {
			XmlWriterSettings result = new XmlWriterSettings();
			result.CloseOutput = false;
			result.ConformanceLevel = ConformanceLevel.Fragment;
			result.Encoding = Encoding.Unicode;
			result.Indent = false;
			result.OmitXmlDeclaration = true;
			return result;
		}

		private object GetValueInternal(int ordinal) {
			int fieldIndex;
			if (schema.TryGetFieldIndexOfColumn(ordinal, out fieldIndex)) {
				return data[fieldIndex];
			}
			return DBNull.Value;
		}

		private void LoadData(object instance) {
			if ((schema.Fields.Length == 1) && (schema.Fields[0] == null)) {
				data = new[] {schema.Converters[0].ProcessToDb(instance) ?? DBNull.Value};
			} else {
				if (instance == null) {
					data = new object[schema.Fields.Length];
				} else {
					data = FormatterServices.GetObjectData(instance, schema.Fields);
				}
				Debug.Assert(schema.Converters.Length == data.Length);
				for (int i = 0; i < data.Length; i++) {
					MemberConverter converter = schema.Converters[i];
					data[i] = ((converter != null) ? converter.ProcessToDb(data[i]) : data[i]) ?? DBNull.Value;
				}
			}
		}

		private void SetValueInternal(int ordinal, object value) {
			int fieldIndex;
			if (!schema.TryGetFieldIndexOfColumn(ordinal, out fieldIndex)) {
				throw new ArgumentOutOfRangeException("ordinal");
			}
			data[fieldIndex] = value ?? DBNull.Value;
		}
	}
}

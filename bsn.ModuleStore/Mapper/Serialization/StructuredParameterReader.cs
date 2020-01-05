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
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace bsn.ModuleStore.Mapper.Serialization {
	internal class StructuredParameterReader: DbDataReader {
		private readonly object[] data;
		private readonly IEnumerator enumerator;
		private readonly StructuredParameterSchemaBase schema;
		private bool isClosed;
		private int rowCount;

		public StructuredParameterReader(StructuredParameterSchemaBase schema, IEnumerable values) {
			if (schema == null) {
				throw new ArgumentNullException(nameof(schema));
			}
			this.schema = schema;
			data = new object[schema.MappedColumns.Count];
			if (values != null) {
				enumerator = values.GetEnumerator();
				if (!enumerator.MoveNext()) {
					(enumerator as IDisposable)?.Dispose();
					enumerator = null;
				}
			}
		}

		public override object this[int ordinal] => GetValue(ordinal);

		public override object this[string name] => this[GetOrdinal(name)];

		public override int Depth => 0;

		public override int FieldCount => schema.ColumnCount;

		public override bool HasRows => (enumerator != null);

		public override bool IsClosed => isClosed;

		public override int RecordsAffected => rowCount;

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
				var value = GetValueInternal(i);
				if (value is XmlReader reader) {
					value = new MemoryStream();
					CopyXmlReaderContent(reader, XmlWriter.Create((Stream)value, CreateXmlWriterSettings()));
					SetValueInternal(i, value);
				}
				switch (value) {
				case byte[] bytes when fieldOffset >= bytes.LongLength:
					return 0;
				case byte[] bytes:
					length = Math.Min((int)Math.Min(bytes.LongLength-fieldOffset, int.MaxValue), length);
					Array.Copy(bytes, fieldOffset, buffer, bufferOffset, length);
					return length;
				case Stream stream: {
					if (stream.Position != fieldOffset) {
						stream.Seek(fieldOffset, SeekOrigin.Begin);
					}
					return stream.Read(buffer, bufferOffset, length);
				}
				}
			}
			return 0;
		}

		public override char GetChar(int i) {
			return (char)GetValueInternal(i);
		}

		public override long GetChars(int i, long fieldOffset, char[] buffer, int bufferOffset, int length) {
			if (length > 0) {
				var value = GetValueInternal(i);
				if (value is XmlReader reader) {
					using (var output = new StringWriter()) {
						CopyXmlReaderContent(reader, XmlWriter.Create(output, CreateXmlWriterSettings()));
						value = output.ToString();
						SetValueInternal(i, value);
					}
				}
				switch (value) {
				case char _ when fieldOffset > 0:
					return 0;
				case char ch:
					buffer[bufferOffset] = ch;
					return 1;
				case string str when fieldOffset >= str.Length:
					return 0;
				case string str: {
					length = Math.Min(str.Length-(int)fieldOffset, length);
					for (i = 0; i < length; i++) {
						buffer[bufferOffset+i] = str[(int)fieldOffset+i];
					}
					return length;
				}
				case char[] chars when fieldOffset >= chars.LongLength:
					return 0;
				case char[] chars:
					length = Math.Min((int)Math.Min(chars.LongLength-fieldOffset, int.MaxValue), length);
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
			for (var i = 0; i < schema.ColumnCount; i++) {
				if (string.Equals(schema.GetColumnName(i), name, StringComparison.OrdinalIgnoreCase)) {
					return i;
				}
			}
			throw new IndexOutOfRangeException($"Could not find the column '{name}'");
		}

		public override DataTable GetSchemaTable() {
			return schema;
		}

		public override string GetString(int i) {
			var value = GetValueInternal(i);
			return (value is string) ? (string)value : value.ToString();
		}

		public override object GetValue(int ordinal) {
			var value = GetValueInternal(ordinal);
			switch (value) {
			case XmlReader reader:
				using (reader) {
					value = new SqlXml(reader);
				}
				SetValueInternal(ordinal, value);
				break;
			}
			return value;
		}

		public override int GetValues(object[] values) {
			if (values == null) {
				throw new ArgumentNullException(nameof(values));
			}
			for (var i = 0; i < schema.ColumnCount; i++) {
				values[i] = GetValue(i);
			}
			return schema.ColumnCount;
		}

		public override bool IsDBNull(int i) {
			if (schema.TryGetFieldIndexOfColumn(i, out var fieldIndex)) {
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
				(enumerator as IDisposable)?.Dispose();
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
			var result = new XmlWriterSettings();
			result.CloseOutput = false;
			result.ConformanceLevel = ConformanceLevel.Fragment;
			result.Encoding = Encoding.Unicode;
			result.Indent = false;
			result.OmitXmlDeclaration = true;
			return result;
		}

		private object GetValueInternal(int ordinal) {
			if (schema.TryGetFieldIndexOfColumn(ordinal, out var fieldIndex)) {
				return data[fieldIndex];
			}
			return DBNull.Value;
		}

		private void LoadData(object instance) {
			if (instance == null) {
				for (var i = 0; i < data.Length; i++) {
					data[i] = DBNull.Value;
				}
			} else if (schema.ExtractMembers != null) {
				schema.ExtractMembers(instance, data);
				for (var i = 0; i < data.Length; i++) {
					var converter = schema.MappedColumns[i].Converter;
					data[i] = ((converter != null) ? converter.ProcessToDb(data[i]) : data[i]) ?? DBNull.Value;
				}
			} else {
				data[0] = schema.MappedColumns[0].Converter.ProcessToDb(instance) ?? DBNull.Value;
			}
		}

		private void SetValueInternal(int ordinal, object value) {
			if (!schema.TryGetFieldIndexOfColumn(ordinal, out var fieldIndex)) {
				throw new ArgumentOutOfRangeException(nameof(ordinal));
			}
			data[fieldIndex] = value ?? DBNull.Value;
		}
	}
}

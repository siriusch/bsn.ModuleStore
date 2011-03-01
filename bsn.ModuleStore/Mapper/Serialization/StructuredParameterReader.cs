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
using System.Diagnostics;
using System.Runtime.Serialization;

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
				return GetValue(ordinal);
			}
		}

		public override object this[string name] {
			get {
				return GetValue(GetOrdinal(name));
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
			Dispose(true);
		}

		public override bool GetBoolean(int i) {
			return (bool)GetValue(i);
		}

		public override byte GetByte(int i) {
			return (byte)GetValue(i);
		}

		public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) {
			throw new NotSupportedException();
		}

		public override char GetChar(int i) {
			return (char)GetValue(i);
		}

		public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) {
			char[] charArray = GetString(i).ToCharArray();
			long charCount = 0;
			for (long k = fieldoffset; k < charArray.Length && charCount < length; k++) {
				buffer[bufferoffset+charCount] = charArray[k];
				charCount++;
			}
			return charCount;
		}

		public override string GetDataTypeName(int i) {
			return schema.GetColumnTypeName(i);
		}

		public override DateTime GetDateTime(int i) {
			return (DateTime)GetValue(i);
		}

		public override decimal GetDecimal(int i) {
			return (decimal)GetValue(i);
		}

		public override double GetDouble(int i) {
			return (double)GetValue(i);
		}

		public override IEnumerator GetEnumerator() {
			throw new NotSupportedException("Iteration over the reader is not supported");
		}

		public override Type GetFieldType(int i) {
			return schema.GetColumnDataType(i);
		}

		public override float GetFloat(int i) {
			return (float)GetValue(i);
		}

		public override Guid GetGuid(int i) {
			return (Guid)GetValue(i);
		}

		public override short GetInt16(int i) {
			return (short)GetValue(i);
		}

		public override int GetInt32(int i) {
			return (int)GetValue(i);
		}

		public override long GetInt64(int i) {
			return (long)GetValue(i);
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
			object value = GetValue(i);
			return (value is string) ? (string)value : value.ToString();
		}

		public override object GetValue(int i) {
			int fieldIndex;
			if (schema.TryGetFieldIndexOfColumn(i, out fieldIndex)) {
				return data[fieldIndex];
			}
			return DBNull.Value;
		}

		public override int GetValues(object[] values) {
			if (values == null) {
				throw new ArgumentNullException("values");
			}
			for (int i = 0; i < schema.ColumnCount; i++) {
				values[i] = GetValue(i);
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

		private void LoadData(object instance) {
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
}

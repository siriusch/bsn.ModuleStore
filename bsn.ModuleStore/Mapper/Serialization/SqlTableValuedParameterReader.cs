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
using System.Linq;

namespace bsn.ModuleStore.Mapper.Serialization {
	internal class SqlTableValuedParameterReader<T>: DbDataReader where T: IEnumerable {
		private static readonly SqlSerializationTypeInfo typeInfo = SqlSerializationTypeInfo.Get(typeof(T));

		private readonly IEnumerator enumerator;
		private bool isClosed;
		private int rowCount;

		public SqlTableValuedParameterReader(IEnumerable values) {
			enumerator = values.GetEnumerator();
			if (!enumerator.MoveNext()) {
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null) {
					disposable.Dispose();
				}
				enumerator = null;
			}
			rowCount = (enumerator != null) ? 0 : -1;
#warning Get rid of this check and warning through runtime binding to parsed data type
			if (typeInfo.Mapping.ListColumns.Any(c => c.Index < 0)) {
				throw new ArgumentException(string.Format("At least one attribute has no 'Index' property specified on type '{0}'", typeof(T).FullName));
			}
			if (typeInfo.Mapping.HasNestedSerializers) {
				throw new NotSupportedException("Nested serialization is not supported for table valued parameters!");
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
				return ListColumns.Length;
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

		private SqlColumnInfo[] ListColumns {
			get {
				return typeInfo.Mapping.ListColumns;
			}
		}

		public override void Close() {
			isClosed = true;
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
			SqlColumnInfo sqlColumnInfo = ListColumns[i];
			if (sqlColumnInfo.DbType == SqlDbType.Udt) {
				return sqlColumnInfo.UserDefinedTypeName;
			}
			return Enum.GetName(typeof(SqlDbType), sqlColumnInfo.DbType);
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
			return enumerator;
		}

		public override Type GetFieldType(int i) {
			return ListColumns[i].ElementType;
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
			return ListColumns[i].Name;
		}

		public override int GetOrdinal(string name) {
			for (int i = 0; i < ListColumns.Length; i++) {
				if (ListColumns[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase)) {
					return i;
				}
			}
			throw new IndexOutOfRangeException(string.Format("Could not find the column '{0}'", name));
		}

		public override DataTable GetSchemaTable() {
			return typeInfo.Mapping.SchemaTable;
		}

		public override string GetString(int i) {
			object value = GetValue(i);
			return (value is string) ? (string)value : value.ToString();
		}

		public override object GetValue(int i) {
			SqlColumnInfo sqlColumnInfo = ListColumns[i];
			object value = sqlColumnInfo.FieldInfo.GetValue(enumerator.Current);
			return sqlColumnInfo.Converter.ProcessToDb(value);
		}

		public override int GetValues(object[] values) {
			int columnCount;
			for (columnCount = 0; ((columnCount < ListColumns.Length) && (columnCount < values.Length)); columnCount++) {
				values[columnCount] = GetValue(columnCount);
			}
			return columnCount;
		}

		public override bool IsDBNull(int i) {
			return (GetValue(i) == DBNull.Value);
		}

		public override bool NextResult() {
			return false;
		}

		public override bool Read() {
			// we alerady performed the first MoveNext() call so we have to return true on the first run (RowCount == 0) and increment rowcount so that the second run is a real MoveNext() call
			return HasRows && ((rowCount++ == 0) || enumerator.MoveNext());
		}
	}
}

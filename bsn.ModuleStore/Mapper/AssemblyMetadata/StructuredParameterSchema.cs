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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

using bsn.ModuleStore.Mapper.Serialization;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Mapper.AssemblyMetadata {
	internal class StructuredParameterSchema: StructuredParameterSchemaBase {
		public StructuredParameterSchema(CreateTypeAsTableStatement script, IDictionary<string, SqlColumnInfo> columnInfos) {
			if (script == null) {
				throw new ArgumentNullException("script");
			}
			if (columnInfos == null) {
				throw new ArgumentNullException("columnInfos");
			}
			SetupColumns(script.ObjectName, script.ObjectSchema);
			DataColumn columnName = Columns[SchemaTableColumn.ColumnName];
			DataColumn allowDbNull = Columns[SchemaTableColumn.AllowDBNull];
			DataColumn dataType = Columns[SchemaTableColumn.DataType];
			DataColumn numericScale = Columns[SchemaTableColumn.NumericScale];
			DataColumn numericPrecision = Columns[SchemaTableColumn.NumericPrecision];
			DataColumn columnSize = Columns[SchemaTableColumn.ColumnSize];
			DataColumn baseColumnName = Columns[SchemaTableColumn.BaseColumnName];
			DataColumn providerType = Columns[SchemaTableColumn.ProviderType];
			DataColumn nonVersionedProviderType = Columns[SchemaTableColumn.NonVersionedProviderType];
			DataColumn columnOrdinal = Columns[SchemaTableColumn.ColumnOrdinal];
			//			if (schema.HasNestedSerializers) {
			//				throw new NotSupportedException("Nested serialization is not supported for table valued parameters!");
			//			}
			List<SqlColumnInfo> sqlColumnInfos = new List<SqlColumnInfo>();
			List<ColumnInfo> columns = new List<ColumnInfo>();
			foreach (TableColumnDefinition column in script.TableDefinitions.OfType<TableColumnDefinition>()) {
				int? fieldIndex = null;
				string columnType = null;
				Type columnDataType = typeof(object);
				DataRow row = NewRow();
				row[columnName] = column.ColumnName.Value;
				bool notNull = column.ColumnDefinition.Constraints.OfType<ColumnNotNullableConstraint>().Any();
				row[allowDbNull] = !notNull;
				SqlColumnInfo info;
				if (columnInfos.TryGetValue(column.ColumnName.Value, out info)) {
					columnDataType = info.Converter.DbClrType;
				} else if (notNull) {
					throw new InvalidOperationException(string.Format("The column {0} on the table type {1} is not nullable, but the re is no matching column for it", column.ColumnName, script.ObjectName));
				} else {
					Debug.Fail(string.Format("No matching field for column {0} of table type {1}", column.ColumnName, script.ObjectName));
				}
				row[dataType] = columnDataType;
				TypedColumnDefinition typedColumn = column.ColumnDefinition as TypedColumnDefinition;
				if (typedColumn != null) {
					columnType = typedColumn.ColumnType.ToString();
					switch (info.DbType) {
					case SqlDbType.Decimal:
						TypeNameWithScale scaledType = typedColumn.ColumnType.Name as TypeNameWithScale;
						if (scaledType != null) {
							row[numericScale] = scaledType.Scale;
						}
						goto case SqlDbType.Float;
					case SqlDbType.Float:
						TypeNameWithPrecision precisionType = typedColumn.ColumnType.Name as TypeNameWithPrecision;
						if (precisionType != null) {
							row[numericPrecision] = precisionType.Precision;
						}
						break;
					case SqlDbType.Binary:
					case SqlDbType.VarBinary:
					case SqlDbType.Char:
					case SqlDbType.VarChar:
					case SqlDbType.NChar:
					case SqlDbType.NVarChar:
						TypeNameNamedExtension extendedType = typedColumn.ColumnType.Name as TypeNameNamedExtension;
						if (extendedType != null) {
							if (string.Equals("MAX", extendedType.Extension.Value, StringComparison.OrdinalIgnoreCase)) {
								row[columnSize] = -1;
							}
						}
						TypeNameWithPrecision lengthType = typedColumn.ColumnType.Name as TypeNameWithPrecision;
						if (lengthType != null) {
							row[columnSize] = lengthType.Precision;
						}
						break;
					}
					fieldIndex = sqlColumnInfos.Count;
					sqlColumnInfos.Add(info);
				}
				row[baseColumnName] = info.Name;
				row[providerType] = (int)info.DbType;
				row[nonVersionedProviderType] = (int)info.DbType;
				row[columnOrdinal] = columns.Count;
				Rows.Add(row);
				columns.Add(new ColumnInfo(fieldIndex, column.ColumnName.Value, columnType, columnDataType));
			}
			MappedColumns = sqlColumnInfos.AsReadOnly();
			ColumnsInfos = columns.ToArray();
			if ((MappedColumns.Count != 1) || (MappedColumns[0].MemberInfo != null)) {
				ExtractMembers = MembersMethods.Get(MappedColumns.Select(c => c.MemberInfo).ToArray()).ExtractMembers;
			}
			AcceptChanges();
		}
	}
}

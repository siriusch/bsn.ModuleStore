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
				throw new ArgumentNullException(nameof(script));
			}
			if (columnInfos == null) {
				throw new ArgumentNullException(nameof(columnInfos));
			}
			SetupColumns(script.ObjectName, script.ObjectSchema);
			var columnName = Columns[SchemaTableColumn.ColumnName];
			var allowDbNull = Columns[SchemaTableColumn.AllowDBNull];
			var dataType = Columns[SchemaTableColumn.DataType];
			var numericScale = Columns[SchemaTableColumn.NumericScale];
			var numericPrecision = Columns[SchemaTableColumn.NumericPrecision];
			var columnSize = Columns[SchemaTableColumn.ColumnSize];
			var baseColumnName = Columns[SchemaTableColumn.BaseColumnName];
			var providerType = Columns[SchemaTableColumn.ProviderType];
			var nonVersionedProviderType = Columns[SchemaTableColumn.NonVersionedProviderType];
			var columnOrdinal = Columns[SchemaTableColumn.ColumnOrdinal];
			//			if (schema.HasNestedSerializers) {
			//				throw new NotSupportedException("Nested serialization is not supported for table valued parameters!");
			//			}
			var sqlColumnInfos = new List<SqlColumnInfo>();
			var columns = new List<ColumnInfo>();
			foreach (var column in script.TableDefinitions.OfType<TableColumnDefinition>()) {
				int? fieldIndex = null;
				string columnType = null;
				var columnDataType = typeof(object);
				var row = NewRow();
				row[columnName] = column.ColumnName.Value;
				var notNull = column.ColumnDefinition.Constraints.OfType<ColumnNotNullableConstraint>().Any();
				row[allowDbNull] = !notNull;
				if (columnInfos.TryGetValue(column.ColumnName.Value, out var info)) {
					columnDataType = info.Converter.DbClrType;
				} else if (notNull) {
					throw new InvalidOperationException($"The column {column.ColumnName} on the table type {script.ObjectName} is not nullable, but the re is no matching column for it");
				} else {
					Debug.Fail($"No matching field for column {column.ColumnName} of table type {script.ObjectName}");
				}
				row[dataType] = columnDataType;
				if (column.ColumnDefinition is TypedColumnDefinition typedColumn) {
					columnType = typedColumn.ColumnType.ToString();
					switch (info.DbType) {
					case SqlDbType.Decimal:
						switch (typedColumn.ColumnType.Name) {
						case TypeNameWithScale scaledType:
							row[numericScale] = scaledType.Scale;
							break;
						}
						goto case SqlDbType.Float;
					case SqlDbType.Float:
						switch (typedColumn.ColumnType.Name) {
						case TypeNameWithPrecision precisionType:
							row[numericPrecision] = precisionType.Precision;
							break;
						}
						break;
					case SqlDbType.Binary:
					case SqlDbType.VarBinary:
					case SqlDbType.Char:
					case SqlDbType.VarChar:
					case SqlDbType.NChar:
					case SqlDbType.NVarChar:
						switch (typedColumn.ColumnType.Name) {
						case TypeNameNamedExtension extendedType when extendedType.IsMax:
							row[columnSize] = -1;
							break;
						case TypeNameWithPrecision lengthType:
							row[columnSize] = lengthType.Precision;
							break;
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

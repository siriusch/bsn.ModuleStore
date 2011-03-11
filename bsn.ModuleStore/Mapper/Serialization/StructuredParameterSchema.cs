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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Mapper.Serialization {
	internal class StructuredParameterSchema: DataTable {
		private struct ColumnInfo {
			private readonly string columnName;
			private readonly Type dataType;
			private readonly int? fieldIndex;
			private readonly string typeName;

			public ColumnInfo(int? fieldIndex, string columnName, string typeName, Type dataType) {
				this.fieldIndex = fieldIndex;
				this.columnName = columnName;
				this.typeName = typeName;
				this.dataType = dataType;
			}

			public string ColumnName {
				get {
					return columnName;
				}
			}

			public Type DataType {
				get {
					return dataType;
				}
			}

			public int? FieldIndex {
				get {
					return fieldIndex;
				}
			}

			public string TypeName {
				get {
					return typeName;
				}
			}
		}

		private readonly ColumnInfo[] columns;
		private readonly MemberConverter[] converters;
		private readonly FieldInfo[] fields;

		public StructuredParameterSchema(CreateTypeAsTableStatement script, IDictionary<string, SqlColumnInfo> columnInfos) {
			if (script == null) {
				throw new ArgumentNullException("script");
			}
			if (columnInfos == null) {
				throw new ArgumentNullException("columnInfos");
			}
			/* The following colums are returned in a data reader created by an query to sql server but they do not exist in the framework column definitions!
			 * schemaTable.Columns.Add("IsIdentity", typeof(Boolean));
			 * schemaTable.Columns.Add("DataTypeName", typeof(String));
			 * schemaTable.Columns.Add("XmlSchemaCollectionDatabase", typeof(String));
			 * schemaTable.Columns.Add("XmlSchemaCollectionOwningSchema", typeof(String));
			 * schemaTable.Columns.Add("XmlSchemaCollectionName", typeof(String));
			 * schemaTable.Columns.Add("UdtAssemblyQualifiedName", typeof(String));
			 * schemaTable.Columns.Add("IsColumnSet", typeof(Boolean));
			 */
			Locale = CultureInfo.InvariantCulture;
			DataColumn columnName = new DataColumn(SchemaTableColumn.ColumnName, typeof(string));
			DataColumn columnOrdinal = new DataColumn(SchemaTableColumn.ColumnOrdinal, typeof(int));
			DataColumn columnSize = new DataColumn(SchemaTableColumn.ColumnSize, typeof(int));
			DataColumn numericPrecision = new DataColumn(SchemaTableColumn.NumericPrecision, typeof(short));
			DataColumn numericScale = new DataColumn(SchemaTableColumn.NumericScale, typeof(short));
			DataColumn dataType = new DataColumn(SchemaTableColumn.DataType, typeof(Type));
			DataColumn providerType = new DataColumn(SchemaTableColumn.ProviderType, typeof(int));
			DataColumn isLong = new DataColumn(SchemaTableColumn.IsLong, typeof(bool));
			DataColumn allowDbNull = new DataColumn(SchemaTableColumn.AllowDBNull, typeof(bool));
			DataColumn isReadOnly = new DataColumn(SchemaTableOptionalColumn.IsReadOnly, typeof(bool));
			DataColumn isRowVersion = new DataColumn(SchemaTableOptionalColumn.IsRowVersion, typeof(bool));
			DataColumn isUnique = new DataColumn(SchemaTableColumn.IsUnique, typeof(bool));
			DataColumn isKey = new DataColumn(SchemaTableColumn.IsKey, typeof(bool));
			DataColumn isAutoIncrement = new DataColumn(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool));
			DataColumn baseSchemaName = new DataColumn(SchemaTableColumn.BaseSchemaName, typeof(string));
			DataColumn baseCatalogName = new DataColumn(SchemaTableOptionalColumn.BaseCatalogName, typeof(string));
			DataColumn baseTableName = new DataColumn(SchemaTableColumn.BaseTableName, typeof(string));
			DataColumn baseColumnName = new DataColumn(SchemaTableColumn.BaseColumnName, typeof(string));
			DataColumn autoIncrementSeed = new DataColumn(SchemaTableOptionalColumn.AutoIncrementSeed, typeof(long));
			DataColumn autoIncrementStep = new DataColumn(SchemaTableOptionalColumn.AutoIncrementStep, typeof(long));
			DataColumn defaultValue = new DataColumn(SchemaTableOptionalColumn.DefaultValue, typeof(object));
			DataColumn expression = new DataColumn(SchemaTableOptionalColumn.Expression, typeof(string));
			DataColumn columnMapping = new DataColumn(SchemaTableOptionalColumn.ColumnMapping, typeof(MappingType));
			DataColumn baseTableNamespace = new DataColumn(SchemaTableOptionalColumn.BaseTableNamespace, typeof(string));
			DataColumn baseColumnNamespace = new DataColumn(SchemaTableOptionalColumn.BaseColumnNamespace, typeof(string));
			DataColumn nonVersionedProviderType = new DataColumn(SchemaTableColumn.NonVersionedProviderType, typeof(int));
			columnSize.DefaultValue = -1;
			baseTableName.DefaultValue = script.ObjectName;
			baseSchemaName.DefaultValue = script.ObjectSchema;
			isRowVersion.DefaultValue = false;
			isLong.DefaultValue = false;
			isReadOnly.DefaultValue = true;
			isKey.DefaultValue = false;
			isAutoIncrement.DefaultValue = false;
			isUnique.DefaultValue = false;
			autoIncrementSeed.DefaultValue = 0;
			autoIncrementStep.DefaultValue = 1;
			Columns.Add(columnName);
			Columns.Add(columnOrdinal);
			Columns.Add(columnSize);
			Columns.Add(numericPrecision);
			Columns.Add(numericScale);
			Columns.Add(dataType);
			Columns.Add(providerType);
			Columns.Add(isLong);
			Columns.Add(allowDbNull);
			Columns.Add(isReadOnly);
			Columns.Add(isRowVersion);
			Columns.Add(isUnique);
			Columns.Add(isKey);
			Columns.Add(isAutoIncrement);
			Columns.Add(baseCatalogName);
			Columns.Add(baseSchemaName);
			Columns.Add(baseTableName);
			Columns.Add(baseColumnName);
			Columns.Add(autoIncrementSeed);
			Columns.Add(autoIncrementStep);
			Columns.Add(defaultValue);
			Columns.Add(expression);
			Columns.Add(columnMapping);
			Columns.Add(baseTableNamespace);
			Columns.Add(baseColumnNamespace);
			Columns.Add(nonVersionedProviderType);
			//			if (schema.HasNestedSerializers) {
			//				throw new NotSupportedException("Nested serialization is not supported for table valued parameters!");
			//			}
			List<SqlColumnInfo> fieldInfos = new List<SqlColumnInfo>();
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
					fieldIndex = fieldInfos.Count;
					fieldInfos.Add(info);
				}
				row[baseColumnName] = info.Name;
				row[providerType] = (int)info.DbType;
				row[nonVersionedProviderType] = (int)info.DbType;
				row[columnOrdinal] = columns.Count;
				Rows.Add(row);
				columns.Add(new ColumnInfo(fieldIndex, column.ColumnName.Value, columnType, columnDataType));
			}
			fields = fieldInfos.Select(i => i.FieldInfo).ToArray();
			converters = fieldInfos.Select(i => i.Converter).ToArray();
			this.columns = columns.ToArray();
			AcceptChanges();
		}

		public int ColumnCount {
			get {
				return columns.Length;
			}
		}

		public MemberConverter[] Converters {
			get {
				return converters;
			}
		}

		public FieldInfo[] Fields {
			get {
				return fields;
			}
		}

		public DbDataReader CreateReader(IEnumerable items) {
			return new StructuredParameterReader(this, items);
		}

		public Type GetColumnDataType(int columnIndex) {
			return columns[columnIndex].DataType;
		}

		public string GetColumnName(int columnIndex) {
			return columns[columnIndex].ColumnName;
		}

		public string GetColumnTypeName(int columnIndex) {
			return columns[columnIndex].TypeName;
		}

		public bool TryGetFieldIndexOfColumn(int columnIndex, out int fieldIndex) {
			if ((columnIndex >= 0) && (columnIndex < columns.Length)) {
				int? result = columns[columnIndex].FieldIndex;
				fieldIndex = result.GetValueOrDefault();
				return result.HasValue;
			}
			fieldIndex = 0;
			return false;
		}
	}
}

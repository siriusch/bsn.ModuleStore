using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

using bsn.ModuleStore.Mapper.Serialization;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	internal class StructuredParameterSchema: StructuredParameterSchemaBase {
		private static string GetDataTypeName(SqlColumnInfo sqlColumnInfo, Type columnType) {
			if (sqlColumnInfo.DbType == SqlDbType.Udt) {
				return SqlCallParameterInfo.GetClrUserDefinedTypeName(columnType);
			}
			return Enum.GetName(typeof(SqlDbType), sqlColumnInfo.DbType);
		}

		public StructuredParameterSchema(ISerializationTypeInfo typeInfo) {
			SetupColumns(typeInfo.Type.Name, typeInfo.Type.Namespace);
			DataColumn columnName = Columns[SchemaTableColumn.ColumnName];
			DataColumn allowDbNull = Columns[SchemaTableColumn.AllowDBNull];
			DataColumn dataType = Columns[SchemaTableColumn.DataType];
/*			DataColumn numericScale = Columns[SchemaTableColumn.NumericScale];
			DataColumn numericPrecision = Columns[SchemaTableColumn.NumericPrecision];
			DataColumn columnSize = Columns[SchemaTableColumn.ColumnSize]; */
			DataColumn baseColumnName = Columns[SchemaTableColumn.BaseColumnName];
			DataColumn providerType = Columns[SchemaTableColumn.ProviderType];
			DataColumn nonVersionedProviderType = Columns[SchemaTableColumn.NonVersionedProviderType];
			DataColumn columnOrdinal = Columns[SchemaTableColumn.ColumnOrdinal];
			List<SqlColumnInfo> sqlColumnInfos = new List<SqlColumnInfo>();
			List<ColumnInfo> columns = new List<ColumnInfo>();
			Dictionary<SqlColumnInfo, int> columnIndices = new Dictionary<SqlColumnInfo, int>();
			if (typeInfo.Mapping.IsNativeType) {
				DataRow row = NewRow();
				row[columnName] = "value";
				row[columnOrdinal] = 0;
				row[dataType] = typeInfo.Type;
				row[allowDbNull] = true;
				row[baseColumnName] = "value";
				row[providerType] = (int)typeInfo.Mapping.DbType;
				Rows.Add(row);
			} else {
				foreach (MemberInfo memberInfo in typeInfo.Type.GetAllFieldsAndProperties()) {
					SqlColumnAttribute sqlColumn = SqlColumnAttributeBase.Get<SqlColumnAttribute>(memberInfo, false);
					if (sqlColumn != null) {
						Type memberType = memberInfo.GetMemberType();
						SqlColumnInfo sqlColumnInfo = typeInfo.Mapping.Columns[sqlColumn.Name];
						StructuredParameterAttribute parameterAttribute = StructuredParameterAttribute.GetStructuredParameterAttribute(memberInfo);
						ColumnInfo ci = new ColumnInfo(sqlColumnInfos.Count, sqlColumnInfo.Name, GetDataTypeName(sqlColumnInfo, memberType), memberType);
						DataRow row = NewRow();
						row[columnName] = ci.ColumnName;
						row[columnOrdinal] = parameterAttribute.Position;
						row[dataType] = ci.DataType;
						row[allowDbNull] = ci.DataType.IsNullableType();
						row[baseColumnName] = ci.ColumnName;
						row[providerType] = (int)sqlColumnInfo.DbType;
						row[nonVersionedProviderType] = (int)sqlColumnInfo.DbType;
						Rows.Add(row);
						columnIndices.Add(sqlColumnInfo, parameterAttribute.Position);
						sqlColumnInfos.Add(sqlColumnInfo);
						columns.Add(ci);
					}
				}
				sqlColumnInfos = sqlColumnInfos.OrderBy(ci => columnIndices[ci]).ToList();
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

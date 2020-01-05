using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
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

		public StructuredParameterSchema(ISerializationTypeInfo typeInfo, ISerializationTypeMappingProvider typeMappingProvider)
		{
			SetupColumns(typeInfo.Type.Name, typeInfo.Type.Namespace);
			var columnName = Columns[SchemaTableColumn.ColumnName];
			var allowDbNull = Columns[SchemaTableColumn.AllowDBNull];
			var dataType = Columns[SchemaTableColumn.DataType];
			/* DataColumn numericScale = Columns[SchemaTableColumn.NumericScale];
			DataColumn numericPrecision = Columns[SchemaTableColumn.NumericPrecision];
			DataColumn columnSize = Columns[SchemaTableColumn.ColumnSize]; */
			var baseColumnName = Columns[SchemaTableColumn.BaseColumnName];
			var providerType = Columns[SchemaTableColumn.ProviderType];
			var nonVersionedProviderType = Columns[SchemaTableColumn.NonVersionedProviderType];
			var columnOrdinal = Columns[SchemaTableColumn.ColumnOrdinal];
			var sqlColumnInfos = new List<SqlColumnInfo>();
			var columns = new List<ColumnInfo>();
			var columnIndices = new Dictionary<SqlColumnInfo, int>();
			if (typeInfo.Mapping.IsNativeType) {
				var row = NewRow();
				row[columnName] = "value";
				row[columnOrdinal] = 0;
				row[dataType] = typeInfo.Type;
				row[allowDbNull] = true;
				row[baseColumnName] = "value";
				row[providerType] = (int)typeInfo.Mapping.DbType;
				Rows.Add(row);
				columns.Add(new ColumnInfo(0, "value", Enum.GetName(typeof(SqlDbType), typeInfo.Mapping.DbType), typeInfo.Type));
				var memberConverter = MemberConverter.Get(typeInfo.Type, false, "value", 0, DateTimeKind.Unspecified);
				sqlColumnInfos.Add(new SqlColumnInfo(typeMappingProvider.GetMapping(typeInfo.Type), "value", memberConverter));
			} else {
				foreach (var memberInfo in typeInfo.Type.GetAllFieldsAndProperties()) {
					var sqlColumn = SqlColumnAttributeBase.Get<SqlColumnAttribute>(memberInfo, false);
					if (sqlColumn != null) {
						var memberType = memberInfo.GetMemberType();
						var sqlColumnInfo = typeInfo.Mapping.Columns[sqlColumn.Name];
						var parameterAttribute = StructuredParameterAttribute.GetStructuredParameterAttribute(memberInfo);
						Debug.Assert(parameterAttribute != null);
						var ci = new ColumnInfo(sqlColumnInfos.Count, sqlColumnInfo.Name, GetDataTypeName(sqlColumnInfo, memberType), memberType);
						var row = NewRow();
						row[columnName] = ci.ColumnName;
						row[columnOrdinal] = parameterAttribute.Position;
						row[dataType] = Nullable.GetUnderlyingType(ci.DataType) ?? ci.DataType;
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
			if ((MappedColumns.Count != 1) ||(MappedColumns[0].MemberInfo != null)) {
				ExtractMembers = MembersMethods.Get(MappedColumns.Select(c => c.MemberInfo).ToArray()).ExtractMembers;
			}
			AcceptChanges();
		}
	}
}

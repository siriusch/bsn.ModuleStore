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
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

using Microsoft.SqlServer.Server;

namespace bsn.ModuleStore.Mapper.Serialization {
	internal class SqlSerializationTypeMapping {
		private static readonly Dictionary<Type, SqlDbType> dbTypeMapping = new Dictionary<Type, SqlDbType> {
		                                                                                                    		{typeof(long), SqlDbType.BigInt},
		                                                                                                    		{typeof(byte[]), SqlDbType.VarBinary},
		                                                                                                    		{typeof(bool), SqlDbType.Bit},
		                                                                                                    		{typeof(char), SqlDbType.NChar},
		                                                                                                    		{typeof(char[]), SqlDbType.NVarChar},
		                                                                                                    		{typeof(string), SqlDbType.NVarChar},
		                                                                                                    		{typeof(DateTime), SqlDbType.DateTime},
		                                                                                                    		{typeof(DateTimeOffset), SqlDbType.DateTimeOffset},
		                                                                                                    		{typeof(decimal), SqlDbType.Decimal},
		                                                                                                    		{typeof(float), SqlDbType.Real},
		                                                                                                    		{typeof(double), SqlDbType.Float},
		                                                                                                    		{typeof(int), SqlDbType.Int},
		                                                                                                    		{typeof(short), SqlDbType.SmallInt},
		                                                                                                    		{typeof(sbyte), SqlDbType.TinyInt},
		                                                                                                    		{typeof(Guid), SqlDbType.UniqueIdentifier}
		                                                                                                    };

		private static readonly Dictionary<Type, SqlSerializationTypeMapping> mappings = new Dictionary<Type, SqlSerializationTypeMapping>();

		internal static bool IsNativeType(Type type) {
			return dbTypeMapping.ContainsKey(type);
		}

		/// <summary>
		/// Get the matching <see cref="DbType"/> for the given type.
		/// </summary>
		/// <param name="type">The .NET type to match.</param>
		/// <returns>The <see cref="DbType"/> best matching the given .NET type, or <see cref="DbType.Object"/> otherwise.</returns>
		public static SqlDbType GetTypeMapping(Type type) {
			if (type != null) {
				if (type.IsByRef && type.HasElementType) {
					type = type.GetElementType();
					Debug.Assert(type != null);
				} else {
					type = Nullable.GetUnderlyingType(type) ?? type;
				}
				SqlDbType result;
				if (dbTypeMapping.TryGetValue(type, out result)) {
					return result;
				}
				if (SqlSerializationTypeInfo.IsXmlType(type)) {
					return SqlDbType.Xml;
				}
				Type dummyType;
				if (SqlSerializationTypeInfo.TryGetIEnumerableElementType(type, out dummyType)) {
					return SqlDbType.Structured;
				}
				if (type.IsDefined(typeof(SqlUserDefinedTypeAttribute), false)) {
					return SqlDbType.Udt;
				}
			}
			return SqlDbType.Variant;
		}

		public static SqlSerializationTypeMapping Get(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			SqlSerializationTypeMapping result;
			lock (mappings) {
				if (!mappings.TryGetValue(type, out result)) {
					result = new SqlSerializationTypeMapping(type);
					mappings.Add(type, result);
				}
			}
			return result;
		}

		internal static IEnumerable<FieldInfo> GetAllFields(Type type) {
			while (type != null) {
				foreach (FieldInfo field in type.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly)) {
					yield return field;
				}
				type = type.BaseType;
			}
		}

		private static DataTable CreateMetadataTable(Type type, IEnumerable<SqlColumnInfo> columnInfos) {
			/* The following colums are returned in a data reader created by an query to sql server but they do not exist in the framework column definitions!
			 * schemaTable.Columns.Add("IsIdentity", typeof(Boolean));
			 * schemaTable.Columns.Add("DataTypeName", typeof(String));
			 * schemaTable.Columns.Add("XmlSchemaCollectionDatabase", typeof(String));
			 * schemaTable.Columns.Add("XmlSchemaCollectionOwningSchema", typeof(String));
			 * schemaTable.Columns.Add("XmlSchemaCollectionName", typeof(String));
			 * schemaTable.Columns.Add("UdtAssemblyQualifiedName", typeof(String));
			 * schemaTable.Columns.Add("IsColumnSet", typeof(Boolean));
			 */
			DataTable schemaTable = new DataTable("SchemaTable");
			schemaTable.Locale = CultureInfo.InvariantCulture;
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
			baseTableName.DefaultValue = type.Name;
			baseTableNamespace.DefaultValue = type.Namespace;
			isRowVersion.DefaultValue = false;
			isLong.DefaultValue = false;
			isReadOnly.DefaultValue = true;
			isKey.DefaultValue = false;
			isAutoIncrement.DefaultValue = false;
			isUnique.DefaultValue = false;
			autoIncrementSeed.DefaultValue = 0;
			autoIncrementStep.DefaultValue = 1;
			schemaTable.Columns.Add(columnName);
			schemaTable.Columns.Add(columnOrdinal);
			schemaTable.Columns.Add(columnSize);
			schemaTable.Columns.Add(numericPrecision);
			schemaTable.Columns.Add(numericScale);
			schemaTable.Columns.Add(dataType);
			schemaTable.Columns.Add(providerType);
			schemaTable.Columns.Add(isLong);
			schemaTable.Columns.Add(allowDbNull);
			schemaTable.Columns.Add(isReadOnly);
			schemaTable.Columns.Add(isRowVersion);
			schemaTable.Columns.Add(isUnique);
			schemaTable.Columns.Add(isKey);
			schemaTable.Columns.Add(isAutoIncrement);
			schemaTable.Columns.Add(baseCatalogName);
			schemaTable.Columns.Add(baseSchemaName);
			schemaTable.Columns.Add(baseTableName);
			schemaTable.Columns.Add(baseColumnName);
			schemaTable.Columns.Add(autoIncrementSeed);
			schemaTable.Columns.Add(autoIncrementStep);
			schemaTable.Columns.Add(defaultValue);
			schemaTable.Columns.Add(expression);
			schemaTable.Columns.Add(columnMapping);
			schemaTable.Columns.Add(baseTableNamespace);
			schemaTable.Columns.Add(baseColumnNamespace);
			schemaTable.Columns.Add(nonVersionedProviderType);
			foreach (SqlColumnInfo info in columnInfos) {
				DataRow row = schemaTable.NewRow();
				row[columnName] = info.Name;
				row[columnOrdinal] = info.Index;
				row[dataType] = info.ElementType;
				row[allowDbNull] = info.IsNullable;
				row[baseColumnName] = info.Name;
				row[providerType] = (int)info.DbType;
				row[nonVersionedProviderType] = (int)info.DbType;
				schemaTable.Rows.Add(row);
			}
			schemaTable.AcceptChanges();
			return schemaTable;
		}

		private readonly ReadOnlyCollection<MemberConverter> converters;
		private readonly FieldInfo[] members;
		private readonly bool hasNestedSerializers;
		private readonly SqlColumnInfo[] listColumns;
		private readonly DataTable schemaTable;

		private SqlSerializationTypeMapping(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			List<MemberConverter> memberConverters = new List<MemberConverter>();
			List<FieldInfo> memberInfos = new List<FieldInfo>();
			List<SqlColumnInfo> columns = new List<SqlColumnInfo>();
			if (!(type.IsPrimitive || type.IsInterface || (typeof(string) == type))) {
				bool hasIdentity = false;
				foreach (FieldInfo field in GetAllFields(type)) {
					SqlColumnAttribute columnAttribute = SqlColumnAttribute.GetColumnAttribute(field, false);
					if (columnAttribute != null) {
						bool isIdentity = (!hasIdentity) && (hasIdentity |= columnAttribute.Identity);
						MemberConverter memberConverter;
						if (columnAttribute.GetCachedByIdentity) {
							memberConverter = new CachedMemberConverter(field.FieldType, isIdentity, columnAttribute.Name, memberInfos.Count, columnAttribute.DateTimeKind);
						} else {
							memberConverter = MemberConverter.Get(field.FieldType, isIdentity, columnAttribute.Name, memberInfos.Count, columnAttribute.DateTimeKind);
						}
						memberConverters.Add(memberConverter);
						memberInfos.Add(field);
						columns.Add(new SqlColumnInfo(field, columnAttribute, memberConverter));
					} else if (field.IsDefined(typeof(SqlDeserializeAttribute), true)) {
						NestedMemberConverter nestedMemberConverter;
						// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
						if (typeof(IList).IsAssignableFrom(field.FieldType)) {
							nestedMemberConverter = new NestedListMemberConverter(field.FieldType, memberInfos.Count);
						} else {
							nestedMemberConverter = new NestedMemberConverter(field.FieldType, memberInfos.Count);
						}
						// ReSharper restore ConvertIfStatementToConditionalTernaryExpression
						memberInfos.Add(field);
						memberConverters.Add(nestedMemberConverter);
						hasNestedSerializers = true;
#warning add support for table valued parameters and SqlDeserializeAttribute (flatten the structure to one "table")
					}
				}
			}
			members = memberInfos.ToArray();
			listColumns = columns.OrderBy(ci => ci.Index).ToArray();
			schemaTable = CreateMetadataTable(type, listColumns);
			converters = Array.AsReadOnly(memberConverters.ToArray());
		}

		public ReadOnlyCollection<MemberConverter> Converters {
			get {
				return converters;
			}
		}

		public int MemberCount {
			get {
				return members.Length;
			}
		}

		public void PopulateInstanceMembers(object result, object[] buffer) {
			FormatterServices.PopulateObjectMembers(result, members, buffer);
		}

		public object GetMember(object instance, int index) {
			return members[index].GetValue(instance);
		}

		public bool HasNestedSerializers {
			get {
				return hasNestedSerializers;
			}
		}

		public SqlColumnInfo[] ListColumns {
			get {
				return listColumns;
			}
		}

		public DataTable SchemaTable {
			get {
				return schemaTable;
			}
		}
	}
}

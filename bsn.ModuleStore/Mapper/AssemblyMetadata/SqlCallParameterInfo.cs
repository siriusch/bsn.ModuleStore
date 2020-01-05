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
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using bsn.ModuleStore.Mapper.Serialization;
using bsn.ModuleStore.Sql;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Mapper.AssemblyMetadata {
	internal class SqlCallParameterInfo: SqlCallParameterBase {
		private static ParameterDirection GetParameterDirection(ParameterInfo param) {
			if (param == null) {
				throw new ArgumentNullException(nameof(param));
			}
			if (param.ParameterType.IsByRef) {
				if (param.IsOut) {
					return ParameterDirection.Output;
				}
				return ParameterDirection.InputOutput;
			}
			return ParameterDirection.Input;
		}

		private static bool GetParameterEnumerable(ParameterInfo param) {
			return param.ParameterType.TryGetIEnumerableElementType(out var structuredType);
		}

		private static bool GetParameterNullable(ParameterInfo param) {
			var parameterType = param.ParameterType;
			if (parameterType.IsByRef) {
				parameterType = parameterType.GetElementType();
				Debug.Assert(parameterType != null);
			}
#warning Maybe implement a non-null attribute on the parameter?
			return (!parameterType.IsValueType) || (System.Nullable.GetUnderlyingType(parameterType) != null);
		}

		private readonly ParameterInfo parameterInfo;
		private readonly StructuredParameterSchema structuredSchema;

		public SqlCallParameterInfo(ISerializationTypeInfoProvider serializationTypeInfoProvider, ParameterInfo param, ProcedureName procedureName, ProcedureParameter script, ISerializationTypeMappingProvider typeMappingProvider)
				: base(procedureName, script, GetParameterDirection(param), GetParameterNullable(param), GetParameterEnumerable(param)) {
			parameterInfo = param;
			if (SqlType == SqlDbType.Structured) {
				if (!param.ParameterType.TryGetIEnumerableElementType(out var structuredType)) {
					throw new ArgumentException("The given parameter must implement IEnumerable<> in order to be used as SQL structured parameter");
				}
				if (!AssemblyInventory.Get(param.Member.DeclaringType.Assembly).TryFind(script.ParameterTypeName.Name.Value, out CreateTypeAsTableStatement createTableTypeScript)) {
					throw new ArgumentException($"The given structured parameter table type {script.ParameterTypeName} cannot be found in the inventory");
				}
				IDictionary<string, SqlColumnInfo> columnInfos;
				var info = serializationTypeInfoProvider.GetSerializationTypeInfo(structuredType, false);
				if (info.SimpleConverter != null) {
					var columnName = createTableTypeScript.TableDefinitions.OfType<TableColumnDefinition>().First(d => d.ColumnDefinition is TypedColumnDefinition).ColumnName.Value;
					columnInfos = new Dictionary<string, SqlColumnInfo>(1);
					columnInfos.Add(columnName, new SqlColumnInfo(typeMappingProvider.GetMapping(structuredType), columnName, info.SimpleConverter));
				} else {
					columnInfos = typeMappingProvider.GetMapping(structuredType).Columns;
					var mappingAttributes = Attribute.GetCustomAttributes(param, typeof(SqlMappingAttribute), true);
					if ((mappingAttributes != null) && (mappingAttributes.Length > 0)) {
						IDictionary<string, SqlColumnInfo> mappedColumnInfos = new Dictionary<string, SqlColumnInfo>(columnInfos);
						foreach (SqlMappingAttribute mappingAttribute in mappingAttributes) {
							mappedColumnInfos[mappingAttribute.TableTypeColumn] = columnInfos[mappingAttribute.SqlColumn];
						}
						columnInfos = mappedColumnInfos;
					}
				}
				structuredSchema = new StructuredParameterSchema(createTableTypeScript, columnInfos);
			}
#warning Maybe implement more type compatibility checks for arguments here
			//			if ((sqlType == SqlDbType.Udt) && string.IsNullOrEmpty(arg.UserDefinedTypeName)) {
			//				userDefinedTypeName = SqlSerializationTypeMapping.GetClrUserDefinedTypeName(parameter.ParameterType, arg);
			//			}
		}

		public string ParameterName => parameterInfo.Name;

		protected override int GetOutArgIndex() {
			return parameterInfo.Position;
		}

		protected override object SetParameterValue(IMethodCallMessage mcm, IList<IDisposable> disposeList) {
			var value = mcm.GetArg(parameterInfo.Position);
			if (value != null) {
				switch (SqlType) {
				case SqlDbType.Xml:
					if (!(value is XmlReader reader)) {
						var navigator = value as XPathNavigator;
						if (navigator == null) {
							switch (value) {
							case IXPathNavigable navigable:
								navigator = navigable.CreateNavigator();
								break;
							case XNode node:
								navigator = node.CreateNavigator();
								break;
							}
							if (navigator == null) {
								throw new NotSupportedException($"XML could not be retrieved from value of type {value.GetType()}.");
							}
						}
						reader = navigator.ReadSubtree();
						disposeList.Add(reader);
					}
					value = new SqlXml(reader);
					break;
				case SqlDbType.SmallInt:
					if (value is IIdentifiable<short> identifiableShort) {
						value = identifiableShort.Id;
					}
					break;
				case SqlDbType.Int:
					if (value is IIdentifiable<int> identifiableInt) {
						value = identifiableInt.Id;
					}
					break;
				case SqlDbType.BigInt:
					if (value is IIdentifiable<long> identifiableLong) {
						value = identifiableLong.Id;
					}
					break;
				case SqlDbType.UniqueIdentifier:
					if (value is IIdentifiable<Guid> identifiableGuid) {
						value = identifiableGuid.Id;
					}
					break;
					//				case SqlDbType.Udt:
					//					parameter.UdtTypeName = userDefinedTypeName;
					//					break;
				}
			}
			if (SqlType == SqlDbType.Structured) {
				IDataReader dataReader = new StructuredParameterReader(structuredSchema, (IEnumerable)value);
				disposeList.Add(dataReader);
				value = dataReader;
			}
			return value;
		}
	}
}

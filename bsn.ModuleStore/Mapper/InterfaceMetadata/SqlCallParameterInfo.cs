// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2011 by Arsène von Wyss - avw@gmx.ch
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
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using Microsoft.SqlServer.Server;

using bsn.ModuleStore.Mapper.Serialization;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	internal class SqlCallParameterInfo {
		internal static string GetClrUserDefinedTypeName(Type type) {
			//string schemaName = ((argInfo != null) && !String.IsNullOrEmpty(argInfo.SchemaName)) ? argInfo.SchemaName.Replace("[", "").Replace("]", "") : "dbo";
#warning check schemaname handling on GetClrUserDefinedTypeName
			var schemaName = "dbo";
			var clrType = type.IsNullableType() ? type.GetGenericArguments()[0] : type;
			var attributes = (SqlUserDefinedTypeAttribute[])clrType.GetCustomAttributes(typeof(SqlUserDefinedTypeAttribute), false);
			if (attributes.Length > 0) {
				var typeName = attributes[0].Name.Replace("[", "").Replace("]", "");
				return $"[{schemaName}].[{typeName}]";
			}
			return String.Empty;
		}

		private static SqlArgAttribute GetSqlArgAttribute(ParameterInfo parameter) {
			SqlArgAttribute arg = null;
			foreach (SqlArgAttribute attribute in parameter.GetCustomAttributes(typeof(SqlArgAttribute), false)) {
				arg = attribute;
			}
			if (arg == null) {
				arg = new SqlArgAttribute(parameter.Name);
			} else if (string.IsNullOrEmpty(arg.Name)) {
				arg = (SqlArgAttribute)arg.CloneWithName(parameter.Name);
			}
			return arg;
		}

		private static object GetXmlValue(object value, IList<IDisposable> disposeList) {
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
			return new SqlXml(reader);
		}

		private readonly ParameterDirection direction = ParameterDirection.Input;
		private readonly bool nullable;
		private readonly int outArgPosition;
		private readonly string parameterName;
		private readonly Type parameterType;
		private readonly int position;
		private readonly int size;
		private readonly SqlDbType sqlType;
		private readonly StructuredParameterSchemaBase structuredSchema;
		private readonly ISerializationTypeInfoProvider typeInfoProvider;
		private readonly string userDefinedTypeName;

		public SqlCallParameterInfo(ParameterInfo parameter, ISerializationTypeInfoProvider typeInfoProvider, ref int outArgCount) {
			if (parameter == null) {
				throw new ArgumentNullException(nameof(parameter));
			}
			if (typeInfoProvider == null) {
				throw new ArgumentNullException(nameof(typeInfoProvider));
			}
			this.typeInfoProvider = typeInfoProvider;
			var arg = GetSqlArgAttribute(parameter);
			parameterName = arg.Name;
			parameterType = parameter.ParameterType;
			if (parameterType.IsByRef) {
				parameterType = parameterType.GetElementType();
			}
			sqlType = (arg.Type != null) ? arg.Type.Value : typeInfoProvider.TypeMappingProvider.GetMapping(parameter.ParameterType).DbType;
			size = arg.Size;
			if ((size == 0) && ((sqlType == SqlDbType.NChar) || (sqlType == SqlDbType.Char))) {
				size = 1;
			}
			nullable = arg.Nullable;
			if (parameter.ParameterType.IsNullableType()) {
				nullable = true;
			} else if (parameter.ParameterType.IsValueType) {
				nullable = false;
			}
			position = parameter.Position;
			if (parameter.ParameterType.IsByRef) {
				if (sqlType == SqlDbType.Structured) {
					throw new NotSupportedException("Table valued parameters only support readonly inputs!");
				}
				direction = parameter.IsOut ? ParameterDirection.Output : ParameterDirection.InputOutput;
			}
			if (direction != ParameterDirection.Input) {
				outArgPosition = outArgCount++;
			}
			if (sqlType == SqlDbType.Structured) {
				structuredSchema = new StructuredParameterSchema(typeInfoProvider.GetSerializationTypeInfo(parameterType.GetGenericArguments()[0], false), typeInfoProvider.TypeMappingProvider);
			}
			if ((sqlType == SqlDbType.Udt) && string.IsNullOrEmpty(arg.UserDefinedTypeName)) {
				userDefinedTypeName = GetClrUserDefinedTypeName(parameter.ParameterType);
			}
		}

		public int Position => position;

		public SqlParameter GetSqlParameter(SqlCommand command, IMethodCallMessage mcm, SqlParameter[] outArgs, IList<IDisposable> disposeList) {
			var parameter = command.CreateParameter();
			parameter.ParameterName = parameterName;
			parameter.IsNullable = nullable;
			if (size > 0) {
				parameter.Size = size;
			}
			parameter.Direction = direction;
			var value = mcm.GetArg(position);
			switch (sqlType) {
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
			case SqlDbType.Xml:
				value = GetXmlValue(value, disposeList);
				break;
			case SqlDbType.Udt:
				parameter.UdtTypeName = userDefinedTypeName;
				break;
			case SqlDbType.Structured:
				value = GetDataTableValue(value, disposeList);
				break;
			}
			var typeInfo = typeInfoProvider.GetSerializationTypeInfo(parameterType, false);
			if (typeInfo.SimpleConverter != null) {
				value = typeInfo.SimpleConverter.ProcessToDb(value);
			}
			parameter.Value = value ?? DBNull.Value;
			parameter.SqlDbType = sqlType;
			if (direction != ParameterDirection.Input) {
				outArgs[outArgPosition] = parameter;
			}
			return parameter;
		}

		private object GetDataTableValue(object value, IList<IDisposable> disposeList) {
			if (!(value is IEnumerable values)) {
				throw new ArgumentException($"The value passed in for parameter '{parameterName}' is castable to IEnumerable");
			}
			var dataReader = new StructuredParameterReader(structuredSchema, values);
			disposeList.Add(dataReader);
			return dataReader;
		}
	}
}

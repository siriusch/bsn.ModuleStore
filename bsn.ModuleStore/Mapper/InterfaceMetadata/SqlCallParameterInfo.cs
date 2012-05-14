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
			string schemaName = "dbo";
			Type clrType = type.IsNullableType() ? type.GetGenericArguments()[0] : type;
			SqlUserDefinedTypeAttribute[] attributes = (SqlUserDefinedTypeAttribute[])clrType.GetCustomAttributes(typeof(SqlUserDefinedTypeAttribute), false);
			if (attributes.Length > 0) {
				string typeName = attributes[0].Name.Replace("[", "").Replace("]", "");
				return String.Format("[{0}].[{1}]", schemaName, typeName);
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
			XmlReader reader = value as XmlReader;
			if (reader == null) {
				XPathNavigator navigator = value as XPathNavigator;
				if (navigator == null) {
					IXPathNavigable navigable = value as IXPathNavigable;
					if (navigable != null) {
						navigator = navigable.CreateNavigator();
					} else {
						XNode node = value as XNode;
						if (node != null) {
							navigator = node.CreateNavigator();
						}
					}
					if (navigator == null) {
						throw new NotSupportedException(String.Format("XML could not be retrieved from value of type {0}.", value.GetType()));
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
				throw new ArgumentNullException("parameter");
			}
			if (typeInfoProvider == null) {
				throw new ArgumentNullException("typeInfoProvider");
			}
			this.typeInfoProvider = typeInfoProvider;
			SqlArgAttribute arg = GetSqlArgAttribute(parameter);
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
				structuredSchema = new StructuredParameterSchema(typeInfoProvider.GetSerializationTypeInfo(parameterType.GetGenericArguments()[0]), typeInfoProvider.TypeMappingProvider);
			}
			if ((sqlType == SqlDbType.Udt) && string.IsNullOrEmpty(arg.UserDefinedTypeName)) {
				userDefinedTypeName = GetClrUserDefinedTypeName(parameter.ParameterType);
			}
		}

		public int Position {
			get {
				return position;
			}
		}

		public SqlParameter GetSqlParameter(SqlCommand command, IMethodCallMessage mcm, SqlParameter[] outArgs, IList<IDisposable> disposeList) {
			SqlParameter parameter = command.CreateParameter();
			parameter.ParameterName = parameterName;
			parameter.IsNullable = nullable;
			if (size > 0) {
				parameter.Size = size;
			}
			parameter.Direction = direction;
			object value = mcm.GetArg(position);
			switch (sqlType) {
			case SqlDbType.SmallInt:
				IIdentifiable<short> identifiableShort = value as IIdentifiable<short>;
				if (identifiableShort != null) {
					value = identifiableShort.Id;
				}
				break;
			case SqlDbType.Int:
				IIdentifiable<int> identifiableInt = value as IIdentifiable<int>;
				if (identifiableInt != null) {
					value = identifiableInt.Id;
				}
				break;
			case SqlDbType.BigInt:
				IIdentifiable<long> identifiableLong = value as IIdentifiable<long>;
				if (identifiableLong != null) {
					value = identifiableLong.Id;
				}
				break;
			case SqlDbType.UniqueIdentifier:
				IIdentifiable<Guid> identifiableGuid = value as IIdentifiable<Guid>;
				if (identifiableGuid != null) {
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
			ISerializationTypeInfo typeInfo = typeInfoProvider.GetSerializationTypeInfo(parameterType);
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
			IEnumerable values = value as IEnumerable;
			if (values == null) {
				throw new ArgumentException(string.Format("The value passed in for parameter '{0}' is castable to IEnumerable", parameterName));
			}
			StructuredParameterReader dataReader = new StructuredParameterReader(structuredSchema, values);
			disposeList.Add(dataReader);
			return dataReader;
		}
	}
}

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

		private static SqlTableValueParameterAttribute GetSqlTablevalueParameterAttribute(ParameterInfo parameter) {
			SqlTableValueParameterAttribute tvp = null;
			foreach (SqlTableValueParameterAttribute attribute in parameter.GetCustomAttributes(typeof(SqlTableValueParameterAttribute), false)) {
				tvp = attribute;
			}
			return tvp;
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

		private static string GetClrUserDefinedTypeName(Type type, SqlTableValueParameterAttribute tvpAttribute)
		{
			string schemaName = ((tvpAttribute != null) && !String.IsNullOrEmpty(tvpAttribute.SchemaName)) ? tvpAttribute.SchemaName.Replace("[", "").Replace("]", "") : "dbo";
			Type clrType = type.IsNullableType() ? type.GetGenericArguments()[0] : type;
			SqlUserDefinedTypeAttribute[] attributes = (SqlUserDefinedTypeAttribute[])clrType.GetCustomAttributes(typeof(SqlUserDefinedTypeAttribute), false);
			if (attributes.Length > 0) {
				string typeName = attributes[0].Name.Replace("[", "").Replace("]", "");
				return String.Format("[{0}].[{1}]", schemaName, typeName);
			}
			return String.Empty;
		}

		private readonly ParameterDirection direction = ParameterDirection.Input;
		private readonly Type listElementType;
		private readonly bool nullable;
		private readonly int outArgPosition;
		private readonly string parameterName;
		private readonly Type parameterType;
		private readonly int position;
		private readonly int size;
		private readonly SqlDbType sqlType;
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
			if ((sqlType == SqlDbType.Structured) && (listElementType == null)) {
				listElementType = parameterType.GetGenericArguments()[0];
			}
			if ((sqlType == SqlDbType.Udt) && string.IsNullOrEmpty(arg.UserDefinedTypeName)) {
				SqlTableValueParameterAttribute sqlTableValueParameterAttribute = GetSqlTablevalueParameterAttribute(parameter);
				userDefinedTypeName = GetClrUserDefinedTypeName(parameter.ParameterType, sqlTableValueParameterAttribute);
			}
		}

		public int OutArgPosition {
			get {
				return outArgPosition;
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
			//SqlTableValuedParameterReader dataReader = new SqlTableValuedParameterReader(typeInfoProvider.GetSerializationTypeInfo(this.listElementType), values);
			//disposeList.Add(dataReader);
			//return dataReader;
#warning ReEnable TVP support with TableValueParameterReader
			return value;
		}
	}
}

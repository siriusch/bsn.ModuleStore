using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Xml;

using bsn.ModuleStore.Mapper.AssemblyMetadata;
using bsn.ModuleStore.Mapper.Serialization;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	internal class SqlCallProcedureInfo: ICallDeserializationInfo {
		private static SqlProcAttribute GetSqlProcAttribute(MethodInfo method) {
			SqlProcAttribute proc = null;
			foreach (SqlProcAttribute attribute in method.GetCustomAttributes(typeof(SqlProcAttribute), false)) {
				proc = attribute;
			}
			if (proc == null) {
				proc = new SqlProcAttribute(method.Name);
			} else if (string.IsNullOrEmpty(proc.Name)) {
				proc = proc.CloneWithName(method.Name);
			}
			return proc;
		}

		private readonly bool deserializeCallConstructor;
		private readonly bool deserializeReturnNullOnEmptyReader;
		private readonly int deserializeRowLimit;
		private readonly bool deserializeScalar;
		private readonly string name;
		private readonly int outArgCount;
		private readonly SqlCallParameterInfo[] parameters;
		private readonly ISerializationTypeInfo returnTypeInfo;
		private readonly string schemaName;
		private readonly int timeout;
		private readonly bool useReturnValue;
		private readonly ParameterInfo xmlNameTableParameter;

		public SqlCallProcedureInfo(MethodInfo method, ISerializationTypeInfoProvider typeInfoProvider) {
			if (method == null) {
				throw new ArgumentNullException(nameof(method));
			}
			if (typeInfoProvider == null) {
				throw new ArgumentNullException(nameof(typeInfoProvider));
			}
			var procedure = GetSqlProcAttribute(method);
			schemaName = procedure.SchemaName;
			name = procedure.Name;
			timeout = procedure.Timeout;
			deserializeReturnNullOnEmptyReader = procedure.DeserializeReturnNullOnEmptyReader;
			deserializeRowLimit = procedure.DeserializeRowLimit;
			deserializeCallConstructor = procedure.DeserializeCallConstructor;
			var sortedParams = new SortedDictionary<int, SqlCallParameterInfo>();
			outArgCount = 0;
			foreach (var parameterInfo in method.GetParameters()) {
				if ((parameterInfo.GetCustomAttributes(typeof(SqlNameTableAttribute), true).Length > 0) || (typeof(XmlNameTable).IsAssignableFrom(parameterInfo.ParameterType))) {
					if (xmlNameTableParameter == null) {
						xmlNameTableParameter = parameterInfo;
					}
				} else {
					var sqlParameterInfo = new SqlCallParameterInfo(parameterInfo, typeInfoProvider, ref outArgCount);
					sortedParams.Add(parameterInfo.Position, sqlParameterInfo);
				}
			}
			parameters = sortedParams.Select(p => p.Value).ToArray();
			deserializeScalar = procedure.UseReturnValue == SqlReturnValue.Scalar;
			returnTypeInfo = typeInfoProvider.GetSerializationTypeInfo(method.ReturnType, deserializeScalar);
			if ((procedure.UseReturnValue != SqlReturnValue.Auto) || (method.ReturnType != typeof(void))) {
				useReturnValue = (procedure.UseReturnValue == SqlReturnValue.ReturnValue) || ((procedure.UseReturnValue == SqlReturnValue.Auto) && (typeInfoProvider.TypeMappingProvider.GetMapping(method.ReturnType).DbType == SqlDbType.Int));
			}
		}

		public string ProcedureName => string.IsNullOrEmpty(schemaName) ? '['+name+']' : '['+schemaName+"].["+name+']';

		public bool DeserializeCallConstructor => deserializeCallConstructor;

		public bool DeserializeScalar => deserializeScalar;

		public int DeserializeRowLimit => deserializeRowLimit;

		public bool RequireTransaction {
#warning check RequireTransationProperty
			get {
				return false;
			}
		}

		public bool DeserializeReturnNullOnEmptyReader => deserializeReturnNullOnEmptyReader;

		public SqlCommand GetCommand(IMethodCallMessage mcm, SqlConnection connection, out SqlParameter returnParameter, out SqlParameter[] outArgs, out ISerializationTypeInfo procedureReturnTypeInfo, out ICallDeserializationInfo procedureInfo, out XmlNameTable xmlNameTable, IList<IDisposable> disposeList) {
			var result = connection.CreateCommand();
			result.CommandText = ProcedureName;
			result.CommandType = CommandType.StoredProcedure;
			if (timeout > 0) {
				result.CommandTimeout = timeout;
			}
			outArgs = new SqlParameter[outArgCount];
			xmlNameTable = xmlNameTableParameter != null ? (XmlNameTable)mcm.GetArg(xmlNameTableParameter.Position) : null;
			foreach (var factory in parameters) {
				result.Parameters.Add(factory.GetSqlParameter(result, mcm, outArgs, disposeList));
			}
			if (useReturnValue) {
				returnParameter = result.CreateParameter();
				returnParameter.SqlDbType = SqlDbType.Int;
				returnParameter.Direction = ParameterDirection.ReturnValue;
				result.Parameters.Add(returnParameter);
			} else {
				returnParameter = null;
			}
			procedureReturnTypeInfo = returnTypeInfo;
			procedureInfo = this;
			return result;
		}
	}
}

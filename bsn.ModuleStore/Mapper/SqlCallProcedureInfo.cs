using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Xml;

namespace bsn.ModuleStore.Mapper {
	internal class SqlCallProcedureInfo {
		private readonly Dictionary<string, SqlCallParameterInfo> arguments = new Dictionary<string, SqlCallParameterInfo>();
		private readonly int outArgCount;
		private readonly SqlProcAttribute proc;
		private readonly SqlDbType returnType;
		private readonly SqlDeserializer.TypeInfo returnTypeInfo;
		private readonly bool useReturnValue;
		private readonly string xmlNameTableParameter;

		public SqlCallProcedureInfo(MethodInfo method) {
			foreach (SqlProcAttribute attribute in method.GetCustomAttributes(typeof(SqlProcAttribute), false)) {
				proc = attribute;
			}
			if (proc == null) {
				proc = new SqlProcAttribute(method.Name);
			} else if (String.IsNullOrEmpty(proc.Name)) {
				proc = (SqlProcAttribute)proc.CloneWithName(method.Name);
			}
			SortedDictionary<int, ParameterInfo> sortedParams = new SortedDictionary<int, ParameterInfo>();
			foreach (ParameterInfo parameterInfo in method.GetParameters()) {
				if ((parameterInfo.GetCustomAttributes(typeof(SqlNameTableAttribute), true).Length > 0) || (typeof(XmlNameTable).IsAssignableFrom(parameterInfo.ParameterType))) {
					if (xmlNameTableParameter == null) {
						xmlNameTableParameter = parameterInfo.Name;
					}
				} else {
					sortedParams.Add(parameterInfo.Position, parameterInfo);
				}
			}
			bool hasOutArg = false;
			foreach (ParameterInfo parameterInfo in sortedParams.Values) {
				arguments.Add(parameterInfo.Name, new SqlCallParameterInfo(parameterInfo, ref hasOutArg));
			}
			outArgCount = (hasOutArg) ? sortedParams.Count : 0;
			returnTypeInfo = SqlDeserializer.TypeInfo.Get(method.ReturnType);
			if ((proc.UseReturnValue != SqlReturnValue.Auto) || (method.ReturnType != typeof(void))) {
				bool? nullable;
				returnType = SqlArgAttribute.GetTypeMapping(method.ReturnType, out nullable);
				useReturnValue = (proc.UseReturnValue == SqlReturnValue.ReturnValue) || ((proc.UseReturnValue == SqlReturnValue.Auto) && (returnType == SqlDbType.Int));
			}
		}

		public SqlCommand GetCommand(IMethodCallMessage mcm, SqlConnection connection, out SqlParameter returnParameter, out KeyValuePair<SqlParameter, Type>[] outArgs, out SqlDeserializer.TypeInfo returnTypeInfo,
		                            out SqlProcAttribute procInfo, out XmlNameTable xmlNameTable, IList<IDisposable> disposeList) {
			SqlCommand result = connection.CreateCommand();
			result.CommandText = proc.Name;
			result.CommandType = CommandType.StoredProcedure;
			if (proc.Timeout > 0) {
				result.CommandTimeout = proc.Timeout;
			}
			outArgs = new KeyValuePair<SqlParameter, Type>[outArgCount];
			xmlNameTable = null;
			for (int i = 0; i < mcm.ArgCount; i++) {
				string argName = mcm.GetArgName(i);
				object argValue = mcm.GetArg(i);
				if (argName.Equals(xmlNameTableParameter)) {
					xmlNameTable = (XmlNameTable)argValue;
				} else {
					result.Parameters.Add(arguments[argName].GetSqlParameter(result, argValue, outArgs, disposeList));
				}
			}
			if (useReturnValue) {
				returnParameter = result.CreateParameter();
				returnParameter.SqlDbType = returnType;
				returnParameter.Direction = ParameterDirection.ReturnValue;
				result.Parameters.Add(returnParameter);
			} else {
				returnParameter = null;
			}
			returnTypeInfo = this.returnTypeInfo;
			procInfo = proc;
			return result;
		}
	}
}
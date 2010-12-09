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
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Xml;

using bsn.ModuleStore.Sql;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Mapper {
	internal class SqlCallProcedureInfo {
		private static readonly Dictionary<Type, SqlDbType> dbTypeMapping = new Dictionary<Type, SqlDbType>
		                                                                    	{
		                                                                    			{typeof(long), SqlDbType.BigInt},
		                                                                    			{typeof(byte[]), SqlDbType.VarBinary},
		                                                                    			{typeof(bool), SqlDbType.Bit},
		                                                                    			{typeof(char), SqlDbType.NChar},
		                                                                    			{typeof(char[]), SqlDbType.NVarChar},
		                                                                    			{typeof(string), SqlDbType.NVarChar},
		                                                                    			{typeof(DateTime), SqlDbType.DateTime},
		                                                                    			{typeof(DateTimeOffset), SqlDbType.DateTimeOffset}, // not supported in SQL 2005!
		                                                                    			{typeof(decimal), SqlDbType.Decimal},
		                                                                    			{typeof(float), SqlDbType.Real},
		                                                                    			{typeof(double), SqlDbType.Float},
		                                                                    			{typeof(int), SqlDbType.Int},
		                                                                    			{typeof(short), SqlDbType.SmallInt},
		                                                                    			{typeof(sbyte), SqlDbType.TinyInt},
		                                                                    			{typeof(Guid), SqlDbType.UniqueIdentifier}
		                                                                    	};

		internal static bool IsNativeType(Type type) {
			return dbTypeMapping.ContainsKey(type);
		}

		public static SqlDbType GetTypeMapping(Type type) {
			if (type != null) {
				if (type.IsByRef && type.HasElementType) {
					type = type.GetElementType();
					Debug.Assert(type != null);
				}
				type = Nullable.GetUnderlyingType(type) ?? type;
				SqlDbType result;
				if (dbTypeMapping.TryGetValue(type, out result)) {
					return result;
				}
				if (SqlDeserializer.IsXmlType(type)) {
					return SqlDbType.Xml;
				}
			}
			return SqlDbType.Udt;
		}

		private readonly Dictionary<string, SqlCallParameterInfo> arguments = new Dictionary<string, SqlCallParameterInfo>();
		private readonly int outArgCount;
		private readonly SqlProcedureAttribute proc;
		private readonly string procedureName;
		private readonly SqlDbType returnType;
		private readonly SqlDeserializerTypeInfo returnTypeInfo;
		private readonly bool useReturnValue;
		private readonly string xmlNameTableParameter;

		public SqlCallProcedureInfo(MethodInfo method) {
			foreach (SqlProcedureAttribute attribute in method.GetCustomAttributes(typeof(SqlProcedureAttribute), false)) {
				proc = attribute;
			}
			if (proc == null) {
				throw new NotImplementedException(String.Format("The method {0}.{1} has no SQL script attached to it.", method.DeclaringType.FullName, method.Name));
			}
			CreateProcedureStatement createStatement = null;
			using (Stream stream = method.DeclaringType.Assembly.GetManifestResourceStream(proc.ManifestResourceType ?? method.DeclaringType, proc.ManifestResourceName)) {
				if (stream == null) {
					throw new FileNotFoundException(String.Format("The embedded script for the method {0}.{1} could not be found", method.DeclaringType.FullName, method.Name), proc.ManifestResourceName);
				}
				using (TextReader reader = new StreamReader(stream, true)) {
					using (IEnumerator<CreateProcedureStatement> enumerator = ScriptParser.Parse(reader).OfType<CreateProcedureStatement>().GetEnumerator()) {
						if (!enumerator.MoveNext()) {
							throw new InvalidOperationException(String.Format("No CREATE PROCEDURE statement found in script {2} (method {0}.{1}).", method.DeclaringType.FullName, method.Name, proc.ManifestResourceName));
						}
						createStatement = enumerator.Current;
						Debug.Assert(createStatement != null);
						if (enumerator.MoveNext()) {
							throw new InvalidOperationException(String.Format("More than one CREATE PROCEDURE statement was found in script {2} (method {0}.{1}).", method.DeclaringType.FullName, method.Name, proc.ManifestResourceName));
						}
					}
				}
			}
			procedureName = createStatement.ProcedureName.Name.Value;
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
			int index = 0;
			foreach (ParameterInfo parameterInfo in sortedParams.Values) {
				if (index >= createStatement.Parameters.Count) {
					throw new InvalidOperationException(String.Format("The method {0}.{1} has more parameters than its stored procedure", method.DeclaringType.FullName, method.Name));
				}
				try {
					arguments.Add(parameterInfo.Name, new SqlCallParameterInfo(parameterInfo, createStatement.Parameters[index++], ref hasOutArg));
				} catch (InvalidOperationException ex) {
					throw new InvalidOperationException(String.Format("The method parameter {2} in method {0}.{1} is invalid: {3}", method.DeclaringType.FullName, method.Name, parameterInfo.Name, ex.Message));
				}
			}
			if (index < createStatement.Parameters.Count) {
				throw new InvalidOperationException(String.Format("The method {0}.{1} has less parameters than its stored procedure", method.DeclaringType.FullName, method.Name));
			}
			outArgCount = (hasOutArg) ? sortedParams.Count : 0;
			returnTypeInfo = SqlDeserializerTypeInfo.Get(method.ReturnType);
			if ((proc.UseReturnValue != SqlReturnValue.Auto) || (method.ReturnType != typeof(void))) {
				useReturnValue = (proc.UseReturnValue == SqlReturnValue.ReturnValue) || ((proc.UseReturnValue == SqlReturnValue.Auto) && (GetTypeMapping(method.ReturnType) == SqlDbType.Int));
			}
		}

		public string ProcedureName {
			get {
				return procedureName;
			}
		}

		public SqlCommand GetCommand(IMethodCallMessage mcm, SqlConnection connection, string schemaName, out SqlParameter returnParameter, out KeyValuePair<SqlParameter, Type>[] outArgs, out SqlDeserializerTypeInfo returnTypeInfo, out SqlProcedureAttribute procInfo, out XmlNameTable xmlNameTable,
		                             IList<IDisposable> disposeList) {
			if (String.IsNullOrEmpty(schemaName)) {
				throw new ArgumentNullException("schemaName");
			}
			SqlCommand result = connection.CreateCommand();
			result.CommandText = String.Format("[{0}].[{1}]", schemaName, procedureName);
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
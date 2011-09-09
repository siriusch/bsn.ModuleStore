using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Xml;

using bsn.ModuleStore.Mapper.Serialization;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	internal class SqlCallInfo: ISqlCallInfo {
		private static readonly Dictionary<Type, SqlCallInfo> knownTypes = new Dictionary<Type, SqlCallInfo>();

		internal static SqlCallInfo Get(Type interfaceType, ISerializationTypeInfoProvider typeInfoProvider) {
			if (interfaceType == null) {
				throw new ArgumentNullException("interfaceType");
			}
			if (typeInfoProvider == null) {
				throw new ArgumentNullException("typeInfoProvider");
			}
			SqlCallInfo result;
			if (!knownTypes.TryGetValue(interfaceType, out result)) {
				lock (knownTypes) {
					if (!knownTypes.TryGetValue(interfaceType, out result)) {
						result = new SqlCallInfo(interfaceType, typeInfoProvider);
						knownTypes.Add(interfaceType, result);
					}
				}
			}
			return result;
		}

		private readonly Type interfaceType;

		private readonly Dictionary<MethodBase, SqlCallProcedureInfo> methods = new Dictionary<MethodBase, SqlCallProcedureInfo>();

		private SqlCallInfo(Type interfaceType, ISerializationTypeInfoProvider typeInfoProvider) {
			Debug.Assert(interfaceType != null);
			if (interfaceType == null) {
				throw new ArgumentNullException("interfaceType");
			}
			if (typeInfoProvider == null) {
				throw new ArgumentNullException("typeInfoProvider");
			}
			if ((!interfaceType.IsInterface) || (interfaceType.IsGenericTypeDefinition) || (!typeof(IStoredProcedures).IsAssignableFrom(interfaceType))) {
				throw new ArgumentException("interfaceType");
			}
			this.interfaceType = interfaceType;
			foreach (MemberInfo memberInfo in interfaceType.GetMembers()) {
				MethodInfo methodInfo = memberInfo as MethodInfo;
				if (methodInfo == null) {
					throw new InvalidOperationException("Only methods are supported");
				}
				if (methodInfo.Name != "Dispose") {
					methods.Add(methodInfo, new SqlCallProcedureInfo(methodInfo, typeInfoProvider));
				}
			}
		}

		public IEnumerable<SqlCommand> CreateCommands(IMethodCallMessage mcm, SqlConnection connection, string schemaName, out SqlParameter returnValue, out SqlParameter[] outParameters, out ISerializationTypeInfo returnTypeInfo, out ICallDeserializationInfo procInfo, out XmlNameTable xmlNameTable,
		                                              IList<IDisposable> disposeList) {
			List<SqlCommand> sqlCommands = new List<SqlCommand>();
			SqlCommand sqlCommand = methods[mcm.MethodBase].GetCommand(mcm, connection, out returnValue, out outParameters, out returnTypeInfo, out procInfo, out xmlNameTable, disposeList);
			sqlCommands.Add(sqlCommand);
			return sqlCommands;
		}

		public string GetProcedureName(IMethodCallMessage mcm, string schemaName) {
			return methods[mcm.MethodBase].ProcedureName;
		}

		public IMessage HandleException(IMethodCallMessage mcm, SqlException exception) {
#warning Check handleException Implementation
			return new ReturnMessage(exception, mcm);
		}

		public Type InterfaceType {
			get {
				return interfaceType;
			}
		}
	}
}

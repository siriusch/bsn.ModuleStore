using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Xml;

namespace bsn.ModuleStore.Mapper {
	internal class SqlCallInfo {
		private static readonly Dictionary<Type, SqlCallInfo> knownTypes = new Dictionary<Type, SqlCallInfo>();

		public static SqlCallInfo Get(Type interfaceType) {
			if (interfaceType == null) {
				throw new ArgumentNullException("interfaceType");
			}
			lock (knownTypes) {
				SqlCallInfo result;
				if (!knownTypes.TryGetValue(interfaceType, out result)) {
					result = new SqlCallInfo(interfaceType);
					knownTypes.Add(interfaceType, result);
				}
				return result;
			}
		}

		private readonly Type interfaceType;
		private readonly Dictionary<MethodBase, SqlCallProcedureInfo> methods = new Dictionary<MethodBase, SqlCallProcedureInfo>();

		private SqlCallInfo(Type interfaceType) {
			Debug.Assert(interfaceType != null);
			if ((!interfaceType.IsInterface) || (interfaceType.IsGenericTypeDefinition) || (!typeof(IDisposable).IsAssignableFrom(interfaceType))) {
				throw new ArgumentException("interfaceType");
			}
			this.interfaceType = interfaceType;
			foreach (MemberInfo memberInfo in interfaceType.GetMembers()) {
				MethodInfo methodInfo = memberInfo as MethodInfo;
				if (methodInfo == null) {
					throw new InvalidOperationException("Only methods are supported");
				}
				if (methodInfo.Name != "Dispose") {
					methods.Add(methodInfo, new SqlCallProcedureInfo(methodInfo));
				}
			}
		}

		public Type InterfaceType {
			get {
				return interfaceType;
			}
		}

		public SqlCommand CreateCommand(IMethodCallMessage mcm, SqlConnection connection, string schemaName, out SqlParameter returnValue, out KeyValuePair<SqlParameter, Type>[] outParameters, out SqlDeserializer.TypeInfo returnTypeInfo, out SqlProcedureAttribute procInfo, out XmlNameTable xmlNameTable,
		                                IList<IDisposable> disposeList) {
			return methods[mcm.MethodBase].GetCommand(mcm, connection, schemaName, out returnValue, out outParameters, out returnTypeInfo, out procInfo, out xmlNameTable, disposeList);
		}
	}
}
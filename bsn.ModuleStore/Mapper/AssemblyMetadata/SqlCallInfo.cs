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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Xml;

using bsn.ModuleStore.Mapper.Serialization;
using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Mapper.AssemblyMetadata {
	internal class SqlCallInfo: ISqlCallInfo {
		private readonly Type interfaceType;
		private readonly Dictionary<MethodBase, SqlCallProcedureInfo> methods = new Dictionary<MethodBase, SqlCallProcedureInfo>();

		internal SqlCallInfo(AssemblyInventory inventory, ISerializationTypeInfoProvider serializationTypeInfoProvider, Type interfaceType) {
			if (inventory == null) {
				throw new ArgumentNullException("inventory");
			}
			if (serializationTypeInfoProvider == null) {
				throw new ArgumentNullException("serializationTypeInfoProvider");
			}
			Debug.Assert(interfaceType != null);
			if ((!interfaceType.IsInterface) || (interfaceType.IsGenericTypeDefinition) || (!typeof(IStoredProcedures).IsAssignableFrom(interfaceType))) {
				throw new ArgumentException("The interface must inherit from IStoredProcedures", "interfaceType");
			}
			this.interfaceType = interfaceType;
			foreach (Type innerInterface in interfaceType.GetInterfaces()) {
				if (innerInterface != typeof(IStoredProcedures)) {
					throw new ArgumentException("The interface cannot inherit from other interfaces then IStoredProcedures", "interfaceType");
				}
			}
			foreach (MemberInfo memberInfo in interfaceType.GetMembers(BindingFlags.Instance|BindingFlags.Public|BindingFlags.DeclaredOnly)) {
				MethodInfo methodInfo = memberInfo as MethodInfo;
				if (methodInfo == null) {
					throw new ArgumentException("Only methods are supported on the IStoredProcedures interfaces", "interfaceType");
				}
				methods.Add(methodInfo, new SqlCallProcedureInfo(inventory, serializationTypeInfoProvider, methodInfo));
			}
		}

		public Type InterfaceType {
			get {
				return interfaceType;
			}
		}

		public IEnumerable<SqlCommand> CreateCommands(IMethodCallMessage mcm, SqlConnection connection, string schemaName, out SqlParameter returnValue, out SqlParameter[] outParameters, out ISerializationTypeInfo returnTypeInfo, out ICallDeserializationInfo procInfo, out XmlNameTable xmlNameTable,
		                                              IList<IDisposable> disposeList) {
			return methods[mcm.MethodBase].GetCommands(mcm, connection, schemaName, out returnValue, out outParameters, out returnTypeInfo, out procInfo, out xmlNameTable, disposeList);
		}

		public string GetProcedureName(IMethodCallMessage mcm, string schemaName) {
			string procedureName = methods[mcm.MethodBase].ProcedureName;
			return string.IsNullOrEmpty(schemaName) ? '['+procedureName+']' : '['+schemaName+"].["+procedureName+']';
		}

		public IMessage HandleException(IMethodCallMessage mcm, SqlException exception) {
			return new ReturnMessage(methods[mcm.MethodBase].MapException(exception), mcm);
		}
	}
}

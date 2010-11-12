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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Xml;
using System.Xml.XPath;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// The DbCallProxy class allows the creation of transparent proxies to call stored procedures with typed and named parameters, and return results in different ways.
	/// <br/><br/>
	/// The interface can declare any number of methods, which are then used to call stored procedures on the database.
	/// <br/><br/>
	/// See also <see cref="SqlProcedureAttribute"/> for bindings on methods.
	/// <br/><br/>
	/// The result is returned as follows:<br/>
	/// - When no return value is expected (void), the call is made as non-query call (<see cref="DbCommand.ExecuteNonQuery"/>).<br/>
	/// - When the return value is a <see cref="IDataReader"/>, a DataReader is opened and returned (<see cref="DbCommand.ExecuteReader(CommandBehavior)"/>). Note that this return type cannot be combined with OUTPUT arguments.<br/>
	/// - When the return value is an interface implementing <see cref="ITypedDataReader"/>, a typed data reader is initialized. Note that this return type cannot be combined with OUTPUT arguments.<br/>
	/// - When the return value is a <see cref="XPathNavigator"/>, a XPathNavigator will be returned to navigator over the rows.<br/>
	/// - When the return value is an <see cref="int"/> and the <see cref="SqlReturnValue"/> is <see cref="SqlReturnValue.Auto"/> or <see cref="SqlReturnValue.ReturnValue"/>, the SP result is returned.
	/// - When the return value is another primitive (or <see cref="int"/> with DbReturnValue.<see cref="SqlReturnValue.Scalar"/> set), a scalar call is made (<see cref="DbCommand.ExecuteScalar"/>).
	/// </summary>
	/// <seealso cref="SqlProcedureAttribute"/>
	/// <seealso cref="SqlReturnValue"/>
	/// <seealso cref="IConnectionProvider"/>
	public class SqlCallProxy: RealProxy {
		private static readonly MethodInfo getAssembly = typeof(IStoredProcedures).GetProperty("Assembly").GetGetMethod();
		private static readonly MethodInfo getInstanceName = typeof(IStoredProcedures).GetProperty("InstanceName").GetGetMethod();
		private static readonly MethodInfo getProvider = typeof(IStoredProcedures).GetProperty("Provider").GetGetMethod();
		private static readonly MethodInfo setProvider = typeof(IStoredProcedures).GetProperty("Provider").GetSetMethod();

		/// <summary>
		/// Create a new proxy to be used for stored procedure calls, which can be called through the interface specified by <typeparamref name="I"/>.
		/// </summary>
		/// <typeparam name="I">The interface declaring the calls. This interface must implement <see cref="IStoredProcedures"/></typeparam>
		/// <param name="connectionProvider">The provider for the SQL connections.</param>
		/// <returns>An instance of the type requested by <typeparamref name="I"/>.</returns>
		public static I Create<I>(IConnectionProvider connectionProvider) where I: IStoredProcedures {
			return (I)(new SqlCallProxy(connectionProvider, typeof(I))).GetTransparentProxy();
		}

		private static object[] GetOutArgValues(KeyValuePair<SqlParameter, Type>[] dbParams) {
			if (dbParams == null) {
				return null;
			}
			object[] result = new object[dbParams.Length];
			for (int i = 0; i < result.Length; i++) {
				SqlParameter param = dbParams[i].Key;
				Type valueType = dbParams[i].Value;
				if (param != null) {
					if ((valueType != null) && ((param.Value == null) || (param.Value == DBNull.Value))) {
						result[i] = Activator.CreateInstance(valueType);
					} else {
						result[i] = param.Value;
					}
				}
			}
			return result;
		}

		private readonly SqlCallInfo callInfo;
		private readonly IConnectionProvider connectionProvider;
		private IInstanceProvider provider;

		private SqlCallProxy(IConnectionProvider connectionProvider, Type interfaceToProxy): base(interfaceToProxy) {
			if (connectionProvider == null) {
				throw new ArgumentNullException("connectionProvider");
			}
			this.connectionProvider = connectionProvider;
			callInfo = SqlCallInfo.Get(interfaceToProxy);
		}

		/// <summary>
		/// Handle a method invocation. This is called by the proxy and should not be called explicitly.
		/// </summary>
		public override IMessage Invoke(IMessage msg) {
			IMethodCallMessage mcm = (IMethodCallMessage)msg;
			try {
				if (mcm.MethodBase == getInstanceName) {
					return new ReturnMessage(connectionProvider.SchemaName, null, 0, mcm.LogicalCallContext, mcm);
				}
				if (mcm.MethodBase == getAssembly) {
					return new ReturnMessage(callInfo.InterfaceType.Assembly, null, 0, mcm.LogicalCallContext, mcm);
				}
				if (mcm.MethodBase == getProvider) {
					return new ReturnMessage(provider, null, 0, mcm.LogicalCallContext, mcm);
				}
				if (mcm.MethodBase == setProvider) {
					provider = (IInstanceProvider)mcm.Args[0];
					return new ReturnMessage(null, null, 0, mcm.LogicalCallContext, mcm);
				}
				bool ownsConnection = false;
				SqlConnection connection;
				SqlTransaction transaction = connectionProvider.GetTransaction();
				if (transaction != null) {
					connection = transaction.Connection;
					if (connection == null) {
						throw new InvalidOperationException("The transaction is not associated to a connection");
					}
				} else {
					connection = connectionProvider.GetConnection();
					if (connection == null) {
						throw new InvalidOperationException("No connection was returned by the provider");
					}
				}
				SqlDataReader reader = null;
				try {
					ownsConnection = (transaction == null) && (connection.State == ConnectionState.Closed);
					if (ownsConnection) {
						connection.Open();
					} else {
						Debug.Assert(connection.State == ConnectionState.Open);
					}
					SqlParameter returnParameter;
					KeyValuePair<SqlParameter, Type>[] outParameters;
					SqlDeserializerTypeInfo returnTypeInfo;
					SqlProcedureAttribute procInfo;
					IList<IDisposable> disposeList = new List<IDisposable>(0);
					XmlNameTable xmlNameTable;
					using (SqlCommand command = callInfo.CreateCommand(mcm, connection, connectionProvider.SchemaName, out returnParameter, out outParameters, out returnTypeInfo, out procInfo, out xmlNameTable, disposeList)) {
						command.Transaction = transaction;
						try {
							Type returnType = ((MethodInfo)mcm.MethodBase).ReturnType;
							object returnValue;
							if (returnType == typeof(void)) {
								command.ExecuteNonQuery();
								returnValue = null;
							} else {
								if ((returnType == typeof(XPathNavigator)) || (returnType == typeof(XPathDocument))) {
									using (XmlReader xmlReader = command.ExecuteXmlReader()) {
										XPathDocument xmlDocument = new XPathDocument(xmlReader);
										if (returnType == typeof(XPathNavigator)) {
											returnValue = xmlDocument.CreateNavigator();
										} else {
											returnValue = xmlDocument;
										}
									}
								} else if (returnType == typeof(XmlReader)) {
									if (ownsConnection) {
										returnValue = new XmlReaderCloseConnection(command.ExecuteXmlReader(), connection);
									} else {
										returnValue = command.ExecuteXmlReader();
									}
									connection = null;
								} else if (typeof(ResultSet).IsAssignableFrom(returnType)) {
									reader = command.ExecuteReader(CommandBehavior.Default);
									returnValue = Activator.CreateInstance(returnType);
									((ResultSet)returnValue).Load(reader, provider);
									if (reader.NextResult()) {
										throw new InvalidOperationException("The reader contains mroe result sets than expected");
									}
									reader.Dispose();
									reader = null;
									ISqlDeserializationHook hook = returnValue as ISqlDeserializationHook;
									if (hook != null) {
										hook.AfterDeserialization();
									}
								} else {
									bool isTypedDataReader = typeof(ITypedDataReader).IsAssignableFrom(returnType);
									if (isTypedDataReader || typeof(IDataReader).IsAssignableFrom(returnType)) {
										Debug.Assert(returnParameter == null);
										if (outParameters.Length > 0) {
											throw new NotSupportedException("Out arguments cannot be combined with a DataReader return value, because DB output values are only returned after the reader is closed. See remarks section of http://msdn.microsoft.com/en-us/library/system.data.common.dbparameter.direction.aspx");
										}
										reader = command.ExecuteReader(ownsConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default);
										connection = null;
										if (isTypedDataReader) {
											try {
												returnValue = new DataReaderProxy(reader, returnType).GetTransparentProxy();
											} catch {
												reader.Dispose();
												throw;
											}
										} else {
											returnValue = reader;
										}
									} else if (returnParameter == null) {
										reader = command.ExecuteReader(CommandBehavior.SingleResult); // no using() or try...finally required, since the "reader" is already handled below
										if (!reader.HasRows) {
											if (procInfo.DeserializeReturnNullOnEmptyReader) {
												returnValue = null;
											} else if (returnTypeInfo.Type.IsArray) {
												returnValue = Array.CreateInstance(returnTypeInfo.InstanceType, 0);
											} else if (returnTypeInfo.IsCollection) {
												returnValue = Activator.CreateInstance(returnTypeInfo.ListType, 0); // creates an List<InstanceType> with capacity 0
											} else {
												throw new InvalidOperationException("The stored procedure did not return any result, but a result was required for object deserialization");
											}
										} else {
											if (returnTypeInfo.SimpleConverter != null) {
												using (SqlDeserializer.DeserializerContext context = new SqlDeserializer.DeserializerContext(reader, provider, xmlNameTable)) {
													if (returnTypeInfo.IsCollection) {
														IList list = returnTypeInfo.CreateList();
														for (int row = procInfo.DeserializeRowLimit; reader.Read() && (row > 0); row--) {
															list.Add(returnTypeInfo.SimpleConverter.Process(context, 0));
														}
														returnValue = returnTypeInfo.FinalizeList(list);
													} else {
														reader.Read();
														returnValue = returnTypeInfo.SimpleConverter.Process(context, 0);
													}
												}
											} else {
												returnValue = new SqlDeserializer(reader, returnType).DeserializeInternal(procInfo.DeserializeRowLimit, provider, procInfo.DeserializeCallConstructor, xmlNameTable);
											}
										}
										reader.Dispose();
										reader = null;
									} else {
										command.ExecuteNonQuery();
										returnValue = returnParameter.Value;
										if (returnType != typeof(int)) {
											returnValue = Convert.ChangeType(returnValue, returnType);
										}
									}
								}
							}
							return new ReturnMessage(returnValue, GetOutArgValues(outParameters), outParameters.Length, mcm.LogicalCallContext, mcm);
						} finally {
							foreach (IDisposable disposable in disposeList) {
								disposable.Dispose();
							}
						}
					}
				} catch {
					if (reader != null) {
						reader.Dispose();
					}
					throw;
				} finally {
					if (ownsConnection && (connection != null)) {
						connection.Dispose();
					}
				}
			} catch (Exception ex) {
				return new ReturnMessage(ex, mcm);
			}
		}
	}
}
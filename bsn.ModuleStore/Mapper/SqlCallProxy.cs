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

using bsn.ModuleStore.Mapper.Serialization;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// The SqlCallProxy class allows the creation of transparent proxies to call stored procedures with typed and named parameters, and return results in different ways.
	/// <br/><br/>
	/// The interface can declare any number of methods, which are then used to call stored procedures on the database.
	/// <br/><br/>
	/// The result is returned as follows:<br/>
	/// - When no return value is expected (void), the call is made as non-query call (<see cref="DbCommand.ExecuteNonQuery"/>).<br/>
	/// - When the return value is a <see cref="IDataReader"/>, a DataReader is opened and returned (<see cref="DbCommand.ExecuteReader(CommandBehavior)"/>). Note that this return type cannot be combined with OUTPUT arguments.<br/>
	/// - When the return value is an interface implementing <see cref="ITypedDataReader"/>, a typed data reader is initialized. Note that this return type cannot be combined with OUTPUT arguments.<br/>
	/// - When the return value is a <see cref="XPathNavigator"/>, a XPathNavigator will be returned to navigator over the rows.<br/>
	/// - When the return value is an <see cref="int"/> and the <see cref="System.Data.SqlClient.SqlReturnValue"/> is <see cref="System.Data.SqlClient.SqlReturnValue.Auto"/> or <see cref="System.Data.SqlClient.SqlReturnValue.ReturnValue"/>, the SP result is returned.
	/// - When the return value is another primitive (or <see cref="int"/> with <see cref="SqlReturnValue"/>.<see cref="System.Data.SqlClient.SqlReturnValue.Scalar"/> set), a scalar call is made (<see cref="DbCommand.ExecuteScalar"/>).
	/// </summary>
	/// <seealso cref="SqlProcedureAttribute"/>
	/// <seealso cref="SqlReturnValue"/>
	/// <seealso cref="IConnectionProvider"/>
	public class SqlCallProxy: RealProxy {
		private struct Profiler {
			private long call;
			private long fetch;
			private string name;
			private Stopwatch sw;
			private bool transaction;

			[Conditional("DEBUG")]
			public void End() {
				if (sw != null) {
					sw.Stop();
					fetch = sw.ElapsedMilliseconds;
					Debug.WriteLine(string.Format("Call {0}ms -- Fetch {1}ms -- Transaction: {2}", call, fetch, transaction), string.Format("Invoked SP {0}", name));
				}
			}

			[Conditional("DEBUG")]
			public void Fetch() {
				Debug.Assert(sw != null);
				sw.Stop();
				call = sw.ElapsedMilliseconds;
				sw.Reset();
				sw.Start();
			}

			[Conditional("DEBUG")]
			public void Start(string name, bool transaction) {
				this.name = name;
				this.transaction = transaction;
				sw = new Stopwatch();
				sw.Start();
			}
		}

		private static readonly MethodInfo equals = typeof(object).GetMethod("Equals", BindingFlags.Public|BindingFlags.Instance);
		private static readonly MethodInfo getAssembly = typeof(IStoredProcedures).GetProperty("Assembly", BindingFlags.Public|BindingFlags.Instance).GetGetMethod();
		private static readonly MethodInfo getHashCode = typeof(object).GetMethod("GetHashCode", BindingFlags.Public|BindingFlags.Instance);
		private static readonly MethodInfo getInstanceName = typeof(IStoredProcedures).GetProperty("InstanceName", BindingFlags.Public|BindingFlags.Instance).GetGetMethod();
		private static readonly MethodInfo getProvider = typeof(IStoredProcedures).GetProperty("Provider", BindingFlags.Public|BindingFlags.Instance).GetGetMethod();
		private static readonly MethodInfo getType = typeof(object).GetMethod("GetType", BindingFlags.Public | BindingFlags.Instance);
		private static readonly MethodInfo setProvider = typeof(IStoredProcedures).GetProperty("Provider", BindingFlags.Public|BindingFlags.Instance).GetSetMethod();
		private static readonly MethodInfo toString = typeof(object).GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance);

		/// <summary>
		/// Create a new proxy to be used for stored procedure calls, which can be called through the interface specified by <typeparamref name="I"/>.
		/// </summary>
		/// <typeparam name="I">The interface declaring the calls. This interface must implement <see cref="IDisposable"/></typeparam>
		/// <param name="metadataProvider">The metadata provider.</param>
		/// <param name="connectionProvider">The connection provider.</param>
		/// <returns>
		/// An instance of the type requested by <typeparamref name="I"/>.
		/// </returns>
		// ReSharper disable InconsistentNaming
		public static I Create<I>(IMetadataProvider metadataProvider, IConnectionProvider connectionProvider) where I: IStoredProcedures {
			return (I)(new SqlCallProxy(metadataProvider, connectionProvider, typeof(I))).GetTransparentProxy();
		}
		// ReSharper restore InconsistentNaming

		private static object[] GetOutArgValues(SqlParameter[] dbParams) {
			if (dbParams == null) {
				return null;
			}
			object[] result = new object[dbParams.Length];
			for (int i = 0; i < result.Length; i++) {
				SqlParameter sqlParameter = dbParams[i];
				if (sqlParameter != null) {
					object value = sqlParameter.Value;
					result[i] = value == DBNull.Value ? null : value;
				}
			}
			return result;
		}

		private readonly ISqlCallInfo callInfo;
		private readonly IConnectionProvider connectionProvider;
		private readonly Dictionary<MethodBase, Func<IMethodCallMessage, IMessage>> methods = new Dictionary<MethodBase, Func<IMethodCallMessage, IMessage>>(8);
		private IInstanceProvider provider;
		private readonly ISerializationTypeInfoProvider serializationTypeInfoProvider;

		private SqlCallProxy(IMetadataProvider metadataProvider, IConnectionProvider connectionProvider, Type interfaceToProxy): base(interfaceToProxy) {
			if (connectionProvider == null) {
				throw new ArgumentNullException("connectionProvider");
			}
			this.connectionProvider = connectionProvider;
			serializationTypeInfoProvider = metadataProvider.GetSerializationTypeInfoProvider() ?? new StaticSerializationTypeInfoProvider();
			callInfo = metadataProvider.GetCallInfo(interfaceToProxy);
			methods.Add(equals, ProxyEquals);
			methods.Add(getAssembly, ProxyGetAssembly);
			methods.Add(getHashCode, ProxyGetHashCode);
			methods.Add(getInstanceName, ProxyGetInstanceName);
			methods.Add(getProvider, ProxyGetProvider);
			methods.Add(getType, ProxyGetType);
			methods.Add(setProvider, ProxySetProvider);
			methods.Add(toString, ProxyToString);
		}

		/// <summary>
		/// Handle a method invocation. This is called by the proxy and should not be called explicitly.
		/// </summary>
		public override IMessage Invoke(IMessage msg) {
			Profiler profiler = new Profiler();
			IMethodCallMessage mcm = (IMethodCallMessage)msg;
			try {
				Func<IMethodCallMessage, IMessage> method;
				if (methods.TryGetValue(mcm.MethodBase, out method)) {
					return method(mcm);
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
				profiler.Start(callInfo.GetProcedureName(mcm, connectionProvider.SchemaName), transaction != null);
				SqlDataReader reader = null;
				try {
					ownsConnection = (transaction == null) && (connection.State == ConnectionState.Closed);
					if (ownsConnection) {
						connection.Open();
					} else {
						Debug.Assert(connection.State == ConnectionState.Open);
					}
					SqlParameter returnParameter;
					SqlParameter[] outParameters;
					ISerializationTypeInfo returnTypeInfo;
					ICallDeserializationInfo procInfo;
					IList<IDisposable> disposeList = new List<IDisposable>(0);
					XmlNameTable xmlNameTable;
					using (IEnumerator<SqlCommand> commandEnumerator = callInfo.CreateCommands(mcm, connection, connectionProvider.SchemaName, out returnParameter, out outParameters, out returnTypeInfo, out procInfo, out xmlNameTable, disposeList).GetEnumerator()) {
						if (procInfo.RequireTransaction && (transaction == null)) {
							throw new InvalidOperationException("A transaction is required for this stored procedure invocation");
						}
						if (!commandEnumerator.MoveNext()) {
							throw new InvalidOperationException();
						}
						SqlCommand command = commandEnumerator.Current;
						Debug.Assert(command != null);
						while (commandEnumerator.MoveNext()) {
							Debug.WriteLine(command.CommandText, "Pre-call SQL execute");
							command.Transaction = transaction;
							command.ExecuteNonQuery();
							command = commandEnumerator.Current;
							Debug.Assert(command != null);
						}
						command.Transaction = transaction;
						try {
							Type returnType = ((MethodInfo)mcm.MethodBase).ReturnType;
							object returnValue;
							if (returnType == typeof(void)) {
								command.ExecuteNonQuery();
								profiler.Fetch();
								returnValue = null;
							} else {
								if ((returnType == typeof(XPathNavigator)) || (returnType == typeof(XPathDocument))) {
									using (XmlReader xmlReader = command.ExecuteXmlReader()) {
										profiler.Fetch();
										XPathDocument xmlDocument = new XPathDocument(xmlReader);
										returnValue = (returnType == typeof(XPathNavigator)) ? (object)xmlDocument.CreateNavigator() : xmlDocument;
									}
								} else if (returnType == typeof(XmlReader)) {
									returnValue = ownsConnection ? new XmlReaderCloseConnection(command.ExecuteXmlReader(), connection) : command.ExecuteXmlReader();
									profiler.Fetch();
									connection = null;
								} else if (typeof(ResultSet).IsAssignableFrom(returnType)) {
									reader = command.ExecuteReader(CommandBehavior.Default);
									profiler.Fetch();
									using (SqlDeserializationContext context = new SqlDeserializationContext(provider, serializationTypeInfoProvider)) {
										returnValue = Activator.CreateInstance(returnType);
										((ResultSet)returnValue).Load(context, reader);
										if (reader.NextResult()) {
											throw new InvalidOperationException("The reader contains more result sets than expected");
										}
										reader.Dispose();
										reader = null;
										ISqlDeserializationHook hook = returnValue as ISqlDeserializationHook;
										if (hook != null) {
											hook.AfterDeserialization();
										}
									}
								} else {
									bool isTypedDataReader = typeof(ITypedDataReader).IsAssignableFrom(returnType);
									if (isTypedDataReader || typeof(IDataReader).IsAssignableFrom(returnType)) {
										Debug.Assert(returnParameter == null);
										if (outParameters.Length > 0) {
											throw new NotSupportedException("Out arguments cannot be combined with a DataReader return value, because DB output values are only returned after the reader is closed. See remarks section of http://msdn.microsoft.com/en-us/library/system.data.common.dbparameter.direction.aspx");
										}
										reader = command.ExecuteReader(ownsConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default);
										profiler.Fetch();
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
										profiler.Fetch();
										if (!reader.HasRows) {
											if (procInfo.DeserializeReturnNullOnEmptyReader) {
												returnValue = null;
											} else if (returnTypeInfo.Type.IsArray) {
												returnValue = Array.CreateInstance(returnTypeInfo.InstanceType, 0);
											} else if (returnTypeInfo.IsCollection) {
												returnValue = returnTypeInfo.CreateList();
											} else {
												throw new InvalidOperationException("The stored procedure did not return any result, but a result was required for object deserialization");
											}
										} else {
											using (SqlDeserializationContext context = new SqlDeserializationContext(provider, serializationTypeInfoProvider)) {
												if (returnTypeInfo.SimpleConverter != null) {
													DeserializerContext deserializerContext = new DeserializerContext(context, reader, procInfo.DeserializeCallConstructor, xmlNameTable);
													if (returnTypeInfo.IsCollection) {
														IList list = returnTypeInfo.CreateList();
														for (int row = procInfo.DeserializeRowLimit; reader.Read() && (row > 0); row--) {
															list.Add(returnTypeInfo.SimpleConverter.ProcessFromDb(deserializerContext, 0));
														}
														returnValue = returnTypeInfo.FinalizeList(list);
													} else {
														reader.Read();
														returnValue = returnTypeInfo.SimpleConverter.ProcessFromDb(deserializerContext, 0);
													}
												} else {
													returnValue = new SqlDeserializer(context, reader, returnType, false).DeserializeInternal(procInfo.DeserializeRowLimit, procInfo.DeserializeCallConstructor, xmlNameTable);
												}
											}
										}
										reader.Dispose();
										reader = null;
									} else {
										command.ExecuteNonQuery();
										profiler.Fetch();
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
					profiler.End();
				}
			} catch (Exception ex) {
				IMessage result = null;
				SqlException sqlEx = ex as SqlException;
				if (sqlEx != null) {
					result = callInfo.HandleException(mcm, sqlEx);
				}
				return result ?? new ReturnMessage(ex, mcm);
			}
		}

		private IMessage ProxyEquals(IMethodCallMessage mcm) {
			return new ReturnMessage(GetTransparentProxy() == mcm.Args[0], null, 0, mcm.LogicalCallContext, mcm);
		}

		private IMessage ProxyGetAssembly(IMethodCallMessage mcm) {
			return new ReturnMessage(callInfo.InterfaceType.Assembly, null, 0, mcm.LogicalCallContext, mcm);
		}

		private IMessage ProxyGetHashCode(IMethodCallMessage mcm) {
			return new ReturnMessage(GetHashCode(), null, 0, mcm.LogicalCallContext, mcm);
		}

		private IMessage ProxyGetInstanceName(IMethodCallMessage mcm) {
			return new ReturnMessage(connectionProvider.SchemaName, null, 0, mcm.LogicalCallContext, mcm);
		}

		private IMessage ProxyGetProvider(IMethodCallMessage mcm) {
			return new ReturnMessage(provider, null, 0, mcm.LogicalCallContext, mcm);
		}

		private IMessage ProxyGetType(IMethodCallMessage mcm) {
			return new ReturnMessage(callInfo.InterfaceType, null, 0, mcm.LogicalCallContext, mcm);
		}

		private IMessage ProxySetProvider(IMethodCallMessage mcm) {
			provider = (IInstanceProvider)mcm.Args[0];
			return new ReturnMessage(null, null, 0, mcm.LogicalCallContext, mcm);
		}

		private IMessage ProxyToString(IMethodCallMessage mcm) {
			return new ReturnMessage(string.Format("{0}@{1}", callInfo.InterfaceType.FullName, connectionProvider.SchemaName), null, 0, mcm.LogicalCallContext, mcm);
		}
	}
}

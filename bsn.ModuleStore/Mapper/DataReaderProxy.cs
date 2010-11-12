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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// The DataReaderProxy class allows the creation of transparent proxies to access data in a <see cref="IDataReader"/> in a typesafe manner.
	/// </summary>
	public class DataReaderProxy: RealProxy {
		private static readonly string currentPropertyGetter = typeof(ITypedDataReader).GetProperty("Current").GetGetMethod().Name;

		/// <summary>
		/// Create a new typed data reader. The interface returned can then be used to read data in a typesafe manner from the <see cref="IDataReader"/>.
		/// <br/><br/>
		/// For information about the interface declaration, see <see cref="ITypedDataReader"/>.
		/// </summary>
		/// <remarks>The reader will be owned by the ITypedDataReader instance until it is released via <see cref="ITypedDataReader.NextResult"/>. Therefore, it will be closed if the ITypedDataReader instance is disposed before transferring the ownership using NextResult.</remarks>
		/// <typeparam name="I">An interface which implements <see cref="ITypedDataReader"/>.</typeparam>
		/// <param name="reader">The <see cref="IDataReader"/> to use as data source. The proxy will take ownership of the data reader.</param>
		/// <returns>An instance of the type requested by <typeparamref name="I"/>.</returns>
		public static I Create<I>(IDataReader reader) where I: ITypedDataReader {
			return (I)(new DataReaderProxy(reader, typeof(I))).GetTransparentProxy();
		}

		private readonly Dictionary<string, KeyValuePair<int, Type>> columns;
		private readonly IDataReader reader;
		private readonly TypedDataReaderInfo readerInfo;
		private bool disposed;

		internal DataReaderProxy(IDataReader reader, Type type): base(type) {
			if (reader == null) {
				throw new ArgumentNullException("reader");
			}
			this.reader = reader;
			readerInfo = TypedDataReaderInfo.Get(type);
			columns = new Dictionary<string, KeyValuePair<int, Type>>(readerInfo.ColumnCount);
		}

		/// <summary>
		/// Handle a method invocation. This is called by the proxy and should not be called explicitly.
		/// </summary>
		public override IMessage Invoke(IMessage msg) {
			IMethodCallMessage mcm = (IMethodCallMessage)msg;
			try {
				string methodName = mcm.MethodName;
				if (disposed) {
					if (methodName == "Dispose") {
						return new ReturnMessage(null, null, 0, mcm.LogicalCallContext, mcm); // ignore repeated Dispose()
					}
					throw new ObjectDisposedException(GetProxiedType().Name);
				}
				switch (methodName) {
				case "Dispose":
					reader.Dispose();
					disposed = true;
					return new ReturnMessage(null, null, 0, mcm.LogicalCallContext, mcm);
				case "Read":
					return new ReturnMessage(reader.Read(), null, 0, mcm.LogicalCallContext, mcm);
				case "NextResult":
					disposed = reader.NextResult();
					if (disposed) {
						try {
							MethodBase methodBase = mcm.MethodBase;
							if (methodBase.IsGenericMethod) {
								Type[] genericArguments = methodBase.GetGenericArguments();
								if ((genericArguments.Length == 1) && typeof(ITypedDataReader).IsAssignableFrom(genericArguments[0])) {
									return new ReturnMessage(new DataReaderProxy(reader, genericArguments[0]).GetTransparentProxy(), null, 0, mcm.LogicalCallContext, mcm);
								}
							}
						} catch {
							disposed = false;
							throw;
						}
					}
					return new ReturnMessage((disposed) ? reader : null, null, 0, mcm.LogicalCallContext, mcm);
				default:
					object returnValue;
					if (methodName == currentPropertyGetter) {
						returnValue = reader; // the reader implements IDataRecord
					} else {
						KeyValuePair<int, Type> column;
						if (!columns.TryGetValue(methodName, out column)) {
							KeyValuePair<string, Type> columnInfo;
							if (readerInfo.TryGetColumnInfo(methodName, out columnInfo)) {
								column = new KeyValuePair<int, Type>(reader.GetOrdinal(columnInfo.Key), columnInfo.Value);
								columns.Add(methodName, column);
							} else {
								// no matching column found
								throw new NotImplementedException();
							}
						}
						Debug.Assert(column.Value != null);
						returnValue = reader.IsDBNull(column.Key) ? null : reader.GetValue(column.Key);
						if ((returnValue != null) && (returnValue.GetType() != column.Value) && (Nullable.GetUnderlyingType(returnValue.GetType()) == null) && typeof(IConvertible).IsAssignableFrom(returnValue.GetType())) {
							returnValue = Convert.ChangeType(returnValue, column.Value);
						}
					}
					return new ReturnMessage(returnValue, null, 0, mcm.LogicalCallContext, mcm);
				}
			} catch (Exception ex) {
				return new ReturnMessage(ex, mcm);
			}
		}
	}
}

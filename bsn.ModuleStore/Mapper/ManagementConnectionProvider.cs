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

namespace bsn.ModuleStore.Mapper {
	public sealed class ManagementConnectionProvider: IConnectionProvider, IDisposable {
		private readonly bool closeConnection;
		private readonly SqlConnection connection;
		private readonly bool ownsConnection;
		private readonly Stack<string> savepoints = new Stack<string>();
		private readonly string schemaName;
		private readonly object sync = new object();
		private SqlTransaction transaction;

		public ManagementConnectionProvider(string connectionString, string schemaName): this(new SqlConnection(connectionString), schemaName) {
			ownsConnection = true;
		}

		public ManagementConnectionProvider(SqlConnection connection, string schemaName) {
			if (connection == null) {
				throw new ArgumentNullException("connection");
			}
			if (String.IsNullOrEmpty(schemaName)) {
				throw new ArgumentNullException("schemaName");
			}
			this.schemaName = schemaName;
			if (connection.State == ConnectionState.Closed) {
				closeConnection = true;
				try {
					connection.Open();
				} catch {
					connection.Dispose();
					throw;
				}
			}
			this.connection = connection;
		}

		public string DatabaseName {
			get {
				return connection.Database;
			}
		}

		public void BeginTransaction() {
			lock (sync) {
				if (transaction == null) {
					transaction = connection.BeginTransaction();
				} else {
					string savepoint = Guid.NewGuid().ToString("N");
					transaction.Save(savepoint);
					savepoints.Push(savepoint);
				}
			}
		}

		public void EndTransaction(bool commit) {
			lock (sync) {
				if (transaction == null) {
					throw new InvalidOperationException("No open transaction");
				}
				if (savepoints.Count > 0) {
					string savepoint = savepoints.Pop();
					if (!commit) {
						transaction.Rollback(savepoint);
					}
				} else {
					try {
						if (commit) {
							transaction.Commit();
						} else {
							transaction.Rollback();
						}
					} finally {
						try {
							transaction.Dispose();
						} finally {
							transaction = null;
						}
					}
				}
			}
		}

		public string SchemaName {
			get {
				return schemaName;
			}
		}

		public SqlConnection GetConnection() {
			return connection;
		}

		public SqlTransaction GetTransaction() {
			return transaction;
		}

		public void Dispose() {
			if (connection != null) {
				try {
					if (transaction != null) {
						transaction.Dispose();
					}
					if (closeConnection) {
						connection.Close();
					}
				} finally {
					if (ownsConnection) {
						connection.Dispose();
					}
				}
			}
		}
	}
}
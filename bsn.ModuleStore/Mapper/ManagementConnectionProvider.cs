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
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace bsn.ModuleStore.Mapper {
	public sealed class ManagementConnectionProvider: IConnectionProvider, IDisposable {
		private readonly bool closeConnection;
		private readonly SqlConnection connection;
		private readonly DatabaseEngine engine;
		private readonly string engineEdition;
		private readonly string engineLevel;
		private readonly Version engineVersion;
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
			using (SqlCommand cmd = connection.CreateCommand()) {
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "SELECT SERVERPROPERTY('productversion')";
				engineVersion = new Version((string)cmd.ExecuteScalar());
				cmd.CommandText = "SELECT SERVERPROPERTY('productlevel')";
				engineLevel = (string)cmd.ExecuteScalar();
				cmd.CommandText = "SELECT SERVERPROPERTY('edition')";
				engineEdition = (string)cmd.ExecuteScalar();
				if (engineEdition.IndexOf("Azure", StringComparison.OrdinalIgnoreCase) >= 0) {
					engine = DatabaseEngine.SqlAzure;
				} else {
					switch (engineVersion.Major) {
					case 9:
						engine = DatabaseEngine.SqlServer2005;
						break;
					case 10:
						engine = DatabaseEngine.SqlServer2008;
						break;
					case 11:
						engine = DatabaseEngine.SqlServer2010;
						break;
					default:
						engine = DatabaseEngine.Unknown;
						break;
					}
				}
			}
		}

		public string DatabaseName {
			get {
				return connection.Database;
			}
		}

		public DatabaseEngine Engine {
			get {
				return engine;
			}
		}

		public string EngineEdition {
			get {
				return engineEdition;
			}
		}

		public string EngineLevel {
			get {
				return engineLevel;
			}
		}

		public Version EngineVersion {
			get {
				return engineVersion;
			}
		}

		public void BeginTransaction() {
			lock (sync) {
				if (transaction == null) {
					Debug.WriteLine("Starting management transaction");
					transaction = connection.BeginTransaction();
				} else {
					Debug.WriteLine("Creating management transaction savepoint");
					string savepoint = Guid.NewGuid().ToString("N");
					transaction.Save(savepoint);
					savepoints.Push(savepoint);
				}
			}
		}

		public void EndTransaction(bool commit) {
			lock (sync) {
				Debug.WriteLine(string.Format("[Savepoints: {0}] [Commit: {1}]", savepoints.Count, commit), "Ending management transaction");
				if (transaction == null) {
					throw new InvalidOperationException("No open transaction");
				}
				if (savepoints.Count > 0) {
					string savepoint = savepoints.Pop();
					if (!commit) {
						try {
							transaction.Rollback(savepoint);
						} catch (SqlException ex) {
							Trace.WriteLine(ex, "Rollback to savepoint failed");
						}
					}
				} else {
					try {
						if (commit) {
							transaction.Commit();
						} else {
							try {
								transaction.Rollback();
							} catch (SqlException ex) {
								Trace.WriteLine(ex, "Rollback failed");
							}
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

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
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;

using bsn.CommandLine;
using bsn.ModuleStore.Console.Contexts;
using bsn.ModuleStore.Console.Entities;
using bsn.ModuleStore.Mapper;
using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Console {
	internal class ExecutionContext: CommandLineContext<ExecutionContext, ModuleStoreContext>, IDisposable {
		private AssemblyHandler assembly;
		private SqlConnection connection;
		private string schemaName;
		private string serverName = ".";
		private string databaseUser;
		private string databasePassword;

		public ExecutionContext(TextReader input, TextWriter output): base(new ModuleStoreContext(), input, output) {
			// trigger async initialization on app start
			ThreadPool.QueueUserWorkItem(state => ScriptParser.GetSemanticActions());
			connection = new SqlConnection();
		}

		public AssemblyHandler Assembly {
			get {
				return assembly;
			}
			set {
				if (assembly != null) {
					assembly.Dispose();
				}
				assembly = value;
			}
		}

		public bool Connected {
			get {
				return (connection.State == ConnectionState.Open);
			}
		}

		public string Database {
			get {
				return connection.Database;
			}
			set {
				if (string.IsNullOrEmpty(value)) {
					throw new ArgumentNullException();
				}
				connection.ChangeDatabase(value);
			}
		}

		public void SetDatabaseAuthentication(string databaseUser, string databasePassword) {
			Disconnect();
			if (string.IsNullOrEmpty(databaseUser)) {
				this.databaseUser = null;
				this.databasePassword = null;
			} else {
				this.databaseUser = databaseUser;
				this.databasePassword = databasePassword;
			}
		}

		public string DatabaseUser {
			get {
				return databaseUser;
			}
		}

		public string DatabasePassword {
			get {
				return databasePassword;
			}
		}

		public SqlConnection Connection {
			get {
				return connection;
			}
		}

		public string Schema {
			get {
				if (string.IsNullOrEmpty(schemaName)) {
					if (Connected) {
						using (SqlCommand command = connection.CreateCommand()) {
							command.CommandType = CommandType.Text;
							command.CommandText = "SELECT COALESCE(default_schema_name, (SELECT TOP (1) [name] FROM [sys].[schemas] ORDER BY schema_id)) sSchema FROM sys.database_principals WHERE [name] = USER_NAME()";
							return (string)command.ExecuteScalar();
						}
					}
					return string.Empty;
				}
				return schemaName;
			}
			set {
				if (string.IsNullOrEmpty(value)) {
					throw new ArgumentNullException();
				}
				schemaName = value;
			}
		}

		public string ScriptPath {
			get {
				return Directory.GetCurrentDirectory();
			}
			set {
				if (!string.IsNullOrEmpty(value)) {
					Directory.SetCurrentDirectory(value);
				}
			}
		}

		public string Server {
			get {
				if (Connected) {
					return connection.DataSource;
				}
				return serverName;
			}
			set {
				if (value != serverName) {
					if (Connected) {
						throw new InvalidOperationException("Cannot change database server while connected");
					}
					if (string.IsNullOrEmpty(value)) {
						throw new ArgumentNullException("value");
					}
					serverName = value;
				}
			}
		}

		public void Connect(string server, string database, string databaseUser, string databasePassword) {
			Disconnect();
			connection.ConnectionString = BuildConnectionString(server, database, databaseUser, databasePassword);
			connection.Open();
		}

		public void Disconnect() {
			if (connection.State != ConnectionState.Closed) {
				connection.Close();
			}
		}

		public string GetConnectionString() {
			if (Connected) {
				return connection.ConnectionString;
			}
			return BuildConnectionString(Server, Database, DatabaseUser, DatabasePassword);
		}

		public static string BuildConnectionString(string server, string database, string databaseUser, string databasePassword) {
			StringBuilder connectionString = new StringBuilder();
			if (string.IsNullOrEmpty(databaseUser)) {
				connectionString.AppendFormat("Data Source={0};", server);
				if (!string.IsNullOrEmpty(database)) {
					connectionString.AppendFormat("Initial Catalog={0};", database);
				}
				connectionString.Append("Integrated Security=SSPI;");
			} else {
				connectionString.AppendFormat("Server=tcp:{0};", server);
				if (!string.IsNullOrEmpty(database)) {
					connectionString.AppendFormat("Database={0};", database);
				}
				connectionString.AppendFormat("User ID={0};Password={1};Trusted_Connection=False;Encrypt=True;", databaseUser, databasePassword);
			}
			return connectionString.ToString();
		}

		public Inventory GetInventory(Source inventorySource) {
			Inventory inventory;
			switch (inventorySource) {
			case Source.Database:
				using (ManagementConnectionProvider provider = new ManagementConnectionProvider(connection, Schema)) {
					inventory = new DatabaseInventory(provider, Schema);
				}
				break;
			case Source.Files:
				inventory = new ScriptInventory(ScriptPath);
				break;
			case Source.Assembly:
				inventory = new AssemblyInventory(Assembly);
				break;
			default:
				throw new InvalidOperationException("No valid source specified");
			}
			return inventory;
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				Assembly = null;
				Disconnect();
				connection.Dispose();
			}
		}

		void IDisposable.Dispose() {
			GC.SuppressFinalize(this);
			Dispose(true);
		}
	}
}

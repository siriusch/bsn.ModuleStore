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

using Microsoft.SqlServer.Management.Common;

namespace bsn.ModuleStore.Mapper {
	public sealed class SmoConnectionProvider: IConnectionProvider, IDisposable {
		private readonly SqlConnection connection;
		private readonly string schemaName;
		private readonly ServerConnection serverConnection;

		public SmoConnectionProvider(string connectionString, string schemaName): this(new SqlConnection(connectionString), schemaName) {}

		public SmoConnectionProvider(SqlConnection connection, string schemaName) {
			if (connection == null) {
				throw new ArgumentNullException("connection");
			}
			if (connection.State != ConnectionState.Closed) {
				throw new ArgumentException("The connection must be closed", "connection");
			}
			if (String.IsNullOrEmpty(schemaName)) {
				throw new ArgumentNullException("schemaName");
			}
			this.schemaName = schemaName;
			try {
				connection.Open();
				serverConnection = new ServerConnection(connection);
			} catch {
				connection.Dispose();
				throw;
			}
			this.connection = connection;
		}

		public string DatabaseName {
			get {
				return connection.Database;
			}
		}

		public ServerConnection ServerConnection {
			get {
				return serverConnection;
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

		SqlTransaction IConnectionProvider.GetTransaction() {
			return null;
		}

		public void Dispose() {
			if (connection != null) {
				connection.Dispose();
			}
		}
	}
}
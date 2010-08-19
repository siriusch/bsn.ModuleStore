using System;
using System.IO;
using System.Linq;

using bsn.CommandLine;
using bsn.ModuleStore.Console.Contexts;

using Microsoft.SqlServer.Management.Smo;

namespace bsn.ModuleStore.Console {
	internal class ExecutionContext: CommandLineContext<ExecutionContext, ModuleStoreContext> {
		private Server server;
		private Database database;
		private string serverName = ".";
		private string databaseName;
		private string schemaName;

		public ExecutionContext(TextReader input, TextWriter output): base(new ModuleStoreContext(), input, output) {}

		public string Database {
			get {
				return databaseName;
			}
			set {
				if (string.IsNullOrEmpty(value)) {
					throw new ArgumentNullException();
				}
				databaseName = value;
				if (string.IsNullOrEmpty(databaseName)) {
					database = null;
				} else if (server != null) {
					database = server.Databases[value];
				}
			}
		}

		public string Server {
			get {
				return serverName;
			}
			set {
				if (Connected) {
					throw new InvalidOperationException("Cannot change database server while connected");
				}
				if (string.IsNullOrEmpty(value)) {
					throw new ArgumentNullException("value");
				}
				serverName = value;
			}
		}

		public string Schema {
			get {
				if (string.IsNullOrEmpty(schemaName)) {
					if (Connected) {
						return database.DefaultSchema;
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

		public bool Connected {
			get {
				return server != null;
			}
		}

		public Database DatabaseInstance {
			get {
				return database;
			}
		}

		public void Connect() {
			if (server == null) {
				server = new Server(serverName);
				server.ConnectionContext.Connect();
				if (!string.IsNullOrEmpty(databaseName)) {
					database = server.Databases[databaseName];
				}
			}
		}

		public void Disconnect() {
			if (server != null) {
				server.ConnectionContext.Disconnect();
				server = null;
				database = null;
			}
		}
	}
}
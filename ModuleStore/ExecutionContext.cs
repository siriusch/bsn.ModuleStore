using System;
using System.IO;
using System.Linq;
using System.Text;

using bsn.CommandLine;
using bsn.ModuleStore.Console.Contexts;

using Microsoft.SqlServer.Management.Smo;

namespace bsn.ModuleStore.Console {
	internal class ExecutionContext: CommandLineContext<ExecutionContext, ModuleStoreContext>, IDisposable {
		private Server server;
		private Database database;
		private string serverName = ".";
		private string databaseName;
		private string schemaName;
		private AssemblyHandler assembly;

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

		public Database DatabaseInstance {
			get {
				return database;
			}
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

		public string GetConnectionString() {
			StringBuilder connectionString = new StringBuilder();
			connectionString.AppendFormat("Data Source={0};", Server);
			if (!string.IsNullOrEmpty(Database)) {
				connectionString.AppendFormat("Initial Catalog={0};", Database);
			}
			connectionString.Append("Integrated Security=SSPI;");
			return connectionString.ToString();
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				Assembly = null;
				Disconnect();
			}
		}

		void IDisposable.Dispose() {
			GC.SuppressFinalize(this);
			Dispose(true);
		}
	}
}
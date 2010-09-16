using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Bootstrapper;

namespace bsn.ModuleStore.Console.Configurations {
	[NamedItem("server", "Manage the server and database names.")]
	internal class ServerConfiguration: ConfigurationBase<ExecutionContext>, IConfigurationRead<ExecutionContext>, IConfigurationWrite<ExecutionContext> {
		public IEnumerable<ITagItem<ExecutionContext>> GetWriteParameters() {
			yield return new Tag<ExecutionContext, string>("server", "Name of the SQL Server.").SetDefault(context => context.Server);
			yield return new Tag<ExecutionContext, string>("database", "Name of the database on the Server.").SetDefault(context => context.Database).SetOptional(context => !string.IsNullOrEmpty(context.Database));
		}

		public void SetConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			executionContext.Server = (string)parameters["server"];
			executionContext.Database = (string)parameters["database"];
		}

		public IEnumerable<ITagItem<ExecutionContext>> GetReadParameters() {
			yield break;
		}

		public void ShowConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			executionContext.Output.WriteLine("Server: {0}", executionContext.Server);
			executionContext.Output.WriteLine("Database: {0} (exists: {1})", executionContext.Database, executionContext.DatabaseInstance != null);
			executionContext.Output.WriteLine("Connected: {0}", executionContext.Connected);
			string connectionString = executionContext.GetConnectionString();
			executionContext.Output.WriteLine("Connection string: {0}", connectionString);
			if (executionContext.DatabaseInstance != null) {
				executionContext.Output.WriteLine("Database Type: {0}", ModuleDatabase.GetDatabaseType(connectionString));
			}
		}
	}
}
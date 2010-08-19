using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bsn.CommandLine;
using bsn.CommandLine.Context;

namespace bsn.ModuleStore.Console.Configurations {
	[NamedItem("server", "Manage the server and database names.")]
	internal class ServerConfiguration: ConfigurationBase<ExecutionContext>, IConfigurationRead<ExecutionContext>, IConfigurationWrite<ExecutionContext> {
		public IEnumerable<ITagItem> GetWriteParameters() {
			yield return new Tag<string>("server", "Name of the SQL Server.").SetDefault(".");
			yield return new Tag<string>("database", "Name of the database on the Server.");
		}

		public void SetConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			executionContext.Server = (string)parameters["server"];
			executionContext.Database = (string)parameters["database"];
		}

		public IEnumerable<ITagItem> GetReadParameters() {
			yield break;
		}

		public void ShowConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			executionContext.Output.WriteLine("Server: {0}", executionContext.Server);
			executionContext.Output.WriteLine("Database: {0}", executionContext.Database);
			executionContext.Output.WriteLine("Connected: {0}", executionContext.Connected);
		}
	}
}
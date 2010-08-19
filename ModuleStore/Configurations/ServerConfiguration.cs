using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bsn.CommandLine.Context;

namespace bsn.ModuleStore.Console.Configurations {
	internal class ServerConfiguration: ConfigurationBase<ExecutionContext>, IConfigurationRead<ExecutionContext>, IConfigurationWrite<ExecutionContext> {
		public override string Name {
			get {
				return "server";
			}
		}

		public override string Description {
			get {
				return "Manage the server and database names.";
			}
		}

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

		public void WriteConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			executionContext.Output.WriteLine("Server: {0}", executionContext.Server);
			executionContext.Output.WriteLine("Database: {0}", executionContext.Database);
		}
	}
}
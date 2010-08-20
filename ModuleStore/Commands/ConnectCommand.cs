using System;
using System.Collections.Generic;
using System.Linq;

using bsn.CommandLine;
using bsn.CommandLine.Context;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("connect", "Establish the connection to the database server.")]
	internal class ConnectCommand: CommandBase<ExecutionContext> {
		public ConnectCommand(ContextBase<ExecutionContext> owner): base(owner) {}

		public override IEnumerable<ITagItem<ExecutionContext>> GetCommandTags() {
			yield return new Tag<ExecutionContext, string>("server", "The server to connect to.").SetOptional(context => context.Connected);
			yield return new Tag<ExecutionContext, string>("database", "The database on the server.").SetOptional(context => context.DatabaseInstance != null);
		}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			executionContext.Disconnect();
			object value;
			if (tags.TryGetValue("server", out value)) {
				executionContext.Server = (string)value;
			}
			if (tags.TryGetValue("database", out value)) {
				executionContext.Database = (string)value;
			}
			executionContext.Connect();
			executionContext.Output.WriteLine("Connected to database {1} on server {0}", executionContext.Server, executionContext.Database);
		}
	}
}
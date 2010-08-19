using System;
using System.Linq;

using bsn.CommandLine;
using bsn.CommandLine.Context;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("disconnect", "Disconnect from database.")]
	internal class DisconnectCommand: CommandBase<ExecutionContext> {
		public DisconnectCommand(ContextBase<ExecutionContext> owner): base(owner) {}

		public override void Execute(ExecutionContext executionContext, System.Collections.Generic.IDictionary<string, object> tags) {
			executionContext.Disconnect();
			executionContext.Output.WriteLine("Disconnected from server {0}", executionContext.Server);
		}
	}
}
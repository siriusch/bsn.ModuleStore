using System;
using System.Collections.Generic;

using bsn.CommandLine;
using bsn.CommandLine.Context;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("unload", "Unload any assembly previously loaded with the load command.")]
	internal class UnloadCommand: CommandBase<ExecutionContext> {
		public UnloadCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			executionContext.Assembly = null;
		}
	}
}

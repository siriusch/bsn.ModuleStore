using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.CommandLine;
using bsn.CommandLine.Context;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("script", "Create script files for embedding.")]
	internal class ScriptCommand: CommandBase<ExecutionContext> {
		public ScriptCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			throw new NotImplementedException();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.CommandLine;
using bsn.CommandLine.Context;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("difference", "Shows the difference between the database and the scripts")]
	internal class DifferenceCommand: CommandBase<ExecutionContext> {
		public DifferenceCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			throw new NotImplementedException();
		}
	}
}

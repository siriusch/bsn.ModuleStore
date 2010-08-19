using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("dump", "Dump all relevant database objects")]
	internal class DumpCommand: CommandBase<ExecutionContext> {
		public DumpCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			DatabaseInventory inventory = new DatabaseInventory(executionContext.DatabaseInstance, executionContext.Schema);
			inventory.Populate();
			inventory.Dump(null, executionContext.Output);
		}
	}
}

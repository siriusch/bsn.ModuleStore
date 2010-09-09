using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Console.Entities;
using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("uninstall", "Uninstall a schema from a database")]
	internal class UninstallCommand: PerfomingCommandBase {
		public UninstallCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			DatabaseInventory inventory = executionContext.GetInventory(Source.Database) as DatabaseInventory;
			if (inventory == null) {
				throw new NotSupportedException("The database inventory could not be created");
			}
			ExecuteInternal(executionContext, tags, inventory.GenerateUninstallSql());
		}
	}
}

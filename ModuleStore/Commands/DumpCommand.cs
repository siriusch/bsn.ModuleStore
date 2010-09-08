using System;
using System.Collections.Generic;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Console.Entities;
using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("dump", "Dump all relevant database objects")]
	internal class DumpCommand: CommandBase<ExecutionContext> {
		public DumpCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			Inventory inventory;
			Source inventorySource = (Source)tags["source"];
			inventory = executionContext.GetInventory(inventorySource);
			inventory.Populate();
			inventory.Dump(null, executionContext.Output);
		}

		public override IEnumerable<ITagItem<ExecutionContext>> GetCommandTags() {
			yield return new Tag<ExecutionContext, Source>("source", "Source for the dump").SetDefault(context => context.Connected ? Source.Database : Source.Files);
		}
	}
}

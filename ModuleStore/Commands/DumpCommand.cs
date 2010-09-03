using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Console.Entities;
using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("dump", "Dump all relevant database objects")]
	internal class DumpCommand: CommandBase<ExecutionContext> {
		public DumpCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override IEnumerable<ITagItem<ExecutionContext>> GetCommandTags() {
			yield return new Tag<ExecutionContext, Source>("source", "Source for the dump").SetDefault(context => context.Connected ? Source.Database : Source.Files);
		}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			Inventory inventory;
			switch ((Source)tags["source"]) {
			case Source.Database:
				inventory = new DatabaseInventory(executionContext.DatabaseInstance, executionContext.Schema);
				break;
			case Source.Files:
				inventory = new ScriptInventory(executionContext.ScriptPath);
				break;
			case Source.Assembly:
				inventory = new AssemblyInventory(executionContext.Assembly);
				break;
			default:
				throw new InvalidOperationException("No valid source specified");
			}
			inventory.Populate();
			inventory.Dump(null, executionContext.Output);
		}
	}
}

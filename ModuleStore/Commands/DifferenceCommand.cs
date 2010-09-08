using System;
using System.Collections.Generic;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Sql;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("difference", "Shows the difference between the database and the scripts")]
	internal class DifferenceCommand: CommandBase<ExecutionContext> {
		public DifferenceCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			Inventory sourceInventory = executionContext.GetInventory((Entities.Source)tags["source"]);
			Inventory targetInventory = executionContext.GetInventory((Entities.Source)tags["target"]);
			foreach (KeyValuePair<CreateStatement, InventoryObjectDifference> difference in Inventory.Compare(sourceInventory, targetInventory)) {
				executionContext.Output.WriteLine(string.Format(" {0} {1}: {2}", difference.Key.ObjectCategory, difference.Key.ObjectName, difference.Value));
			}
		}

		public override IEnumerable<ITagItem<ExecutionContext>> GetCommandTags() {
			yield return new Tag<ExecutionContext, Entities.Source>("source", "The source for the comparison.").SetDefault(context => context.Assembly != null ? Entities.Source.Assembly : Entities.Source.Files);
			yield return new Tag<ExecutionContext, Entities.Source>("target", "The target for the comparison.").SetDefault(context => context.DatabaseInstance != null ? Entities.Source.Database : Entities.Source.Files);
		}
	}
}

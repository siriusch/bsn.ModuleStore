using System;
using System.Collections.Generic;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Console.Entities;
using bsn.ModuleStore.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("install", "Generate or execute SQL statements to install a source")]
	internal class InstallCommand: CommandBase<ExecutionContext> {
		public InstallCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			Source inventorySource = (Source)tags["source"];
			InstallableInventory inventory = executionContext.GetInventory(inventorySource) as InstallableInventory;
			if (inventory == null) {
				throw new NotSupportedException("The selected inventory type cannot be used as installation source");
			}
			foreach (string sql in inventory.GenerateInstallSql(executionContext.Schema)) {
				executionContext.Output.Write(sql);
				executionContext.Output.WriteLine(";");
				executionContext.Output.WriteLine("GO");
				executionContext.Output.WriteLine();
			}
		}

		public override IEnumerable<ITagItem<ExecutionContext>> GetCommandTags() {
			yield return new Tag<ExecutionContext, Source>("source", "Source for the dump").SetDefault(context => context.Connected ? Source.Database : Source.Files);
		}
	}
}
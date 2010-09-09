using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Console.Entities;
using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("update", "Generate or execute SQL statements to update an assembly source")]
	internal class UpdateCommand: PerfomingCommandBase {
		public UpdateCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			AssemblyInventory assemblyInventory = (AssemblyInventory)executionContext.GetInventory(Source.Assembly);
			if (assemblyInventory == null) {
				throw new NotSupportedException("The assembly inventory could not be created");
			}
			DatabaseInventory databaseInventory = (DatabaseInventory)executionContext.GetInventory(Source.Database);
			if (databaseInventory == null) {
				throw new NotSupportedException("The assembly inventory could not be created");
			}
#warning perform lookup on ModuleStore tables and perform only updates as necessary on all matching schemas
			base.ExecuteInternal(executionContext, tags, assemblyInventory.GenerateUpdateSql(databaseInventory, 0));
		}
	}
}
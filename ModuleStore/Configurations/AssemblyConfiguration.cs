using System;
using System.Collections.Generic;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Console.Configurations {
	[NamedItem("assembly", "Show information about the loaded assembly.")]
	internal class AssemblyConfiguration: ConfigurationBase<ExecutionContext>, IConfigurationRead<ExecutionContext> {
		public IEnumerable<ITagItem<ExecutionContext>> GetReadParameters() {
			yield break;
		}

		public void ShowConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			if (executionContext.Assembly == null) {
				executionContext.Output.WriteLine(" Failed: No assembly loaded.");
				return;
			}
			executionContext.Output.WriteLine(" Assembly: {0}", executionContext.Assembly);
			AssemblyInventory inventory = new AssemblyInventory(executionContext.Assembly);
			inventory.Populate();
			executionContext.Output.WriteLine(" Setup objects: {0}", inventory.Objects.Count);
			executionContext.Output.WriteLine(" Setup statements: {0}", inventory.AdditionalSetupStatements.Count);
			executionContext.Output.WriteLine(" Update scripts: {0}", inventory.UpdateStatements.Count);
		}
	}
}

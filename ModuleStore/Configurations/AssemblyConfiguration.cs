using System;
using System.Collections.Generic;

using bsn.CommandLine;
using bsn.CommandLine.Context;

namespace bsn.ModuleStore.Console.Configurations {
	[NamedItem("assembly", "Show information about the loaded assembly.")]
	internal class AssemblyConfiguration: ConfigurationBase<ExecutionContext>, IConfigurationRead<ExecutionContext> {
		public IEnumerable<ITagItem<ExecutionContext>> GetReadParameters() {
			yield break;
		}

		public void ShowConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			executionContext.Output.WriteLine("Assembly: {0}", executionContext.Assembly);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.CommandLine;
using bsn.CommandLine.Context;

namespace bsn.ModuleStore.Console.Configurations {
	[NamedItem("scripting", "Path for scripting and comparing scripts")]
	internal class ScriptingConfiguration: ConfigurationBase<ExecutionContext>, IConfigurationRead<ExecutionContext>, IConfigurationWrite<ExecutionContext> {
		public IEnumerable<ITagItem<ExecutionContext>> GetReadParameters() {
			yield break;
		}

		public void ShowConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			executionContext.Output.WriteLine("Path: {0}", executionContext.ScriptPath);
		}

		public IEnumerable<ITagItem<ExecutionContext>> GetWriteParameters() {
			yield return new Tag<ExecutionContext, string>("path", "The current scripting path.").SetDefault(context => context.ScriptPath);
		}

		public void SetConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			executionContext.ScriptPath = (string)parameters["path"];
		}
	}
}

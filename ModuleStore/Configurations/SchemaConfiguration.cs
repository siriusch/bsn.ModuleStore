using System;
using System.Collections.Generic;
using System.Linq;

using bsn.CommandLine;
using bsn.CommandLine.Context;

namespace bsn.ModuleStore.Console {
	[NamedItem("schema", "The currently active schema on the database.")]
	internal class SchemaConfiguration: ConfigurationBase<ExecutionContext>, IConfigurationRead<ExecutionContext>, IConfigurationWrite<ExecutionContext> {
		public IEnumerable<ITagItem> GetReadParameters() {
			yield break;
		}

		public void ShowConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			executionContext.Output.WriteLine("Schema: {0}", executionContext.Schema);
		}

		public IEnumerable<ITagItem> GetWriteParameters() {
			yield return new Tag<string>("name", "Database schema name.", false).SetDefault("dbo");
		}

		public void SetConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			executionContext.Schema = (string)parameters["name"];
		}
	}
}
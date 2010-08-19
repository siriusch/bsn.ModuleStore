using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bsn.CommandLine.Context;

namespace bsn.ModuleStore.Console.Configurations {
	public class ServerConfiguration: ConfigurationBase<ExecutionContext>, IConfigurationRead<ExecutionContext>, IConfigurationWrite<ExecutionContext> {
		public override string Name {
			get {
				throw new NotImplementedException();
			}
		}

		public override string Description {
			get {
				throw new NotImplementedException();
			}
		}

		public override void WriteCommandHelp(TextWriter writer) {
			throw new NotImplementedException();
		}

		public IEnumerable<Tag<string>> GetParameters() {
			yield return new Tag<string>("server", "Name of the SQL Server.", false);
			yield return new Tag<string>("database", "Name of the database on the Server.", false);
		}

		public void SetConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {}
	}
}
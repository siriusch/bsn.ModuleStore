using System;
using System.Linq;

using bsn.CommandLine.Context;

namespace bsn.ModuleStore.Console {
	internal class SchemaConfiguration: ConfigurationBase<ExecutionContext> {
		public override string Name {
			get {
				return "schema";
			}
		}

		public override string Description {
			get {
				return "The currently active schema on the database";
			}
		}
	}
}
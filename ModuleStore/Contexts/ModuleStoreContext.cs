using System;
using System.Collections.Generic;

using bsn.CommandLine.Context;
using bsn.ModuleStore.Console.Configurations;

namespace bsn.ModuleStore.Console.Contexts {
	public class ModuleStoreContext: RootContext<ExecutionContext> {
		public ModuleStoreContext(): base("modulestore") {}

		public override IEnumerable<ConfigurationBase<ExecutionContext>> Configurations {
			get {
				yield return new ServerConfiguration();
				yield return new SchemaConfiguration();
			}
		}
	}
}
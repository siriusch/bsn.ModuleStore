using System;

using bsn.CommandLine.Context;

namespace bsn.ModuleStore.Console.Contexts {
	public class ModuleStoreContext: RootContext<ExecutionContext> {
		public ModuleStoreContext(): base("modulestore") {}
	}
}
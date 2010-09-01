using System;
using System.Collections.Generic;

using bsn.CommandLine.Context;
using bsn.ModuleStore.Console.Commands;
using bsn.ModuleStore.Console.Configurations;

namespace bsn.ModuleStore.Console.Contexts {
	internal class ModuleStoreContext: RootContext<ExecutionContext> {
		public ModuleStoreContext(): base("modulestore") {}

		public override IEnumerable<CommandBase<ExecutionContext>> Commands {
			get {
				return Merge(base.Commands, new DumpCommand(this), new DifferenceCommand(this));
			}
		}

		public override IEnumerable<ConfigurationBase<ExecutionContext>> Configurations {
			get {
				return Merge(base.Configurations, new ServerConfiguration(), new SchemaConfiguration(), new ScriptingConfiguration());
			}
		}

		public override IEnumerable<ContextBase<ExecutionContext>> ChildContexts {
			get {
				return Merge(base.ChildContexts, new DatabaseContext(this), new AssemblyContext(this));
			}
		}
	}
}
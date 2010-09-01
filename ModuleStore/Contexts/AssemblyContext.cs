using System;
using System.Collections.Generic;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Console.Commands;
using bsn.ModuleStore.Console.Configurations;

namespace bsn.ModuleStore.Console.Contexts {
	[NamedItem("assembly", "Manage scripts in assemblies.")]
	internal class AssemblyContext: ContextBase<ExecutionContext> {
		public AssemblyContext(ContextBase<ExecutionContext> parentContext): base(parentContext) {}

		public override IEnumerable<CommandBase<ExecutionContext>> Commands {
			get {
				return Merge(base.Commands, new LoadCommand(this), new UnloadCommand(this));
			}
		}

		public override IEnumerable<ConfigurationBase<ExecutionContext>> Configurations {
			get {
				return Merge(base.Configurations, new AssemblyConfiguration());
			}
		}
	}
}

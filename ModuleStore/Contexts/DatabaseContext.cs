using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Console.Commands;
using bsn.ModuleStore.Console.Configurations;

namespace bsn.ModuleStore.Console.Contexts {
	[NamedItem("database", "Database server operations.")]
	internal class DatabaseContext: ContextBase<ExecutionContext> {
		public DatabaseContext(ContextBase<ExecutionContext> parentContext): base(parentContext) {}

		public override IEnumerable<CommandBase<ExecutionContext>> Commands {
			get {
				return Merge(base.Commands, new ConnectCommand(this), new DisconnectCommand(this), new ScriptCommand(this), new UninstallCommand(this), new UpdateCommand(this));
			}
		}

		public override IEnumerable<ConfigurationBase<ExecutionContext>> Configurations {
			get {
				return Merge(base.Configurations, new ServerConfiguration(), new SchemaConfiguration());
			}
		}
	}
}

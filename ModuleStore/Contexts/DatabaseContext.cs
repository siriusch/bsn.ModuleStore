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
				yield return new ConnectCommand(this);
				yield return new DisconnectCommand(this);
			}
		}

		public override IEnumerable<ConfigurationBase<ExecutionContext>> Configurations {
			get {
				yield return new ServerConfiguration();
				yield return new SchemaConfiguration();
			}
		}
	}
}

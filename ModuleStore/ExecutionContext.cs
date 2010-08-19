using System;
using System.IO;
using System.Linq;

using bsn.CommandLine;
using bsn.ModuleStore.Console.Contexts;

namespace bsn.ModuleStore.Console {
	public class ExecutionContext: CommandLineContext<ExecutionContext, ModuleStoreContext> {
		public ExecutionContext(TextReader input, TextWriter output): base(new ModuleStoreContext(), input, output) {}

		public string Database {
			get;
			set;
		}

		public string Server {
			get;
			set;
		}

		public bool Connected {
			get;
			set;
		}
	}
}
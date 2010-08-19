using System;
using System.IO;
using System.Linq;

using bsn.CommandLine;
using bsn.ModuleStore.Console.Contexts;

namespace bsn.ModuleStore.Console {
	public class ExecutionContext: CommandLineContext<ExecutionContext, ModuleStoreContext> {
		public ExecutionContext(TextReader input, TextWriter output): base(new ModuleStoreContext(), input, output) {}
	}
}
using System;

using bsn.CommandLine;
using bsn.ModuleStore.Console;

namespace ModuleStore {
	internal class Program {
		private static void Main() {
			Runner<ExecutionContext> runner = new Runner<ExecutionContext>(new ExecutionContext(Console.In, Console.Out));
			runner.Run();
		}
	}
}
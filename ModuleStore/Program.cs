using System;

using bsn.CommandLine;
using bsn.CommandLine.Context;

namespace ModuleStore {
	internal class Program {
		private static void Main() {
			Runner runner = new Runner(new CommandLineContext<RootContext>(new RootContext("modulestore"), Console.In, Console.Out));
			runner.Run();
		}
	}
}

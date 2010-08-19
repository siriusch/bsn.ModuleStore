using System;
using System.IO;
using System.Linq;

using bsn.CommandLine;
using bsn.ModuleStore.Console.Contexts;

namespace bsn.ModuleStore.Console {
	internal class ExecutionContext: CommandLineContext<ExecutionContext, ModuleStoreContext> {
		private bool connected;
		public ExecutionContext(TextReader input, TextWriter output): base(new ModuleStoreContext(), input, output) {}

		public string Database {
			get;
			set;
		}

		public string Server {
			get;
			set;
		}

		public string Schema {
			get;
			set;
		}

		public bool Connected {
			get {
				return connected;
			}
		}

		public void Connect() {
			connected = true;
		}

		public void Disconnect() {
			connected = false;
		}
	}
}
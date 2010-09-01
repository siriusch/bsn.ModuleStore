using System;
using System.Collections.Generic;
using System.IO;

using bsn.CommandLine;
using bsn.CommandLine.Context;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("load", "Load an assembly.")]
	internal class LoadCommand: CommandBase<ExecutionContext> {
		public LoadCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			executionContext.Assembly = new AssemblyHandler(new FileInfo((string)tags["filename"]));
		}

		public override IEnumerable<ITagItem<ExecutionContext>> GetCommandTags() {
			yield return new Tag<ExecutionContext, string>("filename", "The file name of the assembly to load.");
		}
	}
}

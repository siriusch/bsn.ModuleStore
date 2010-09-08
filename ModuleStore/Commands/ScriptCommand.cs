using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Sql;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("script", "Create script files for embedding.")]
	internal class ScriptCommand: CommandBase<ExecutionContext> {
		public ScriptCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			Encoding encoding = Encoding.GetEncoding((string)tags["encoding"]);
			string basePath = Directory.CreateDirectory(Path.Combine(executionContext.ScriptPath, (string)tags["path"])).FullName;
			executionContext.Output.WriteLine("Scripting to {0} (Encoding: {1})...", basePath, encoding.WebName);
			DatabaseInventory inventory = new DatabaseInventory(executionContext.DatabaseInstance, executionContext.Schema);
			inventory.Populate();
			bool objectDirectories = (bool)tags["directories"];
			Dictionary<string, List<CreateStatement>> objectGroups = new Dictionary<string, List<CreateStatement>>();
			foreach (CreateStatement statement in inventory.Objects) {
				statement.ObjectSchema = (string)tags["schema"];
				try {
					string categoryName = string.Empty;
					if (objectDirectories) {
						statement.ObjectCategory.ToString();
						categoryName += categoryName.EndsWith("x") ? "es\\" : "s\\";
					}
					string fileRelativePath = string.Format("{0}{1}.sql", categoryName, statement.ObjectName);
					executionContext.Output.WriteLine("* Scripting {0}", fileRelativePath);
					FileInfo fileName = new FileInfo(Path.Combine(basePath, fileRelativePath));
					if (fileName.Exists) {
						fileName.Attributes &= ~FileAttributes.ReadOnly;
					} else {
						Directory.CreateDirectory(fileName.DirectoryName);
					}
					using (FileStream stream = fileName.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.Read)) {
						using (StreamWriter writer = new StreamWriter(stream, encoding)) {
							SqlWriter sqlWriter = new SqlWriter(writer);
							statement.WriteTo(sqlWriter);
						}
					}
				} finally {
					statement.ObjectSchema = null;
				}
			}
		}

		public override IEnumerable<ITagItem<ExecutionContext>> GetCommandTags() {
			yield return new Tag<ExecutionContext, string>("path", "The target path for the scripts.").SetDefault(context => context.ScriptPath).SetOptional(context => false);
			yield return new Tag<ExecutionContext, bool>("directories", "If true, each object type is put in a separate directory.").SetDefault(context => false);
			yield return new Tag<ExecutionContext, string>("schema", "The schema to be used in the scripts.").SetDefault(context => string.Empty).SetOptional(context => true);
			yield return new Tag<ExecutionContext, string>("encoding", "The encoding used to save the scripts.").SetDefault(context => "UTF-8").SetOptional(context => true);
		}
	}
}

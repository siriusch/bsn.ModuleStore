using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using System.Linq;

using bsn.ModuleStore.Sql;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("script", "Create script files for embedding.")]
	internal class ScriptCommand: CommandBase<ExecutionContext> {
		public ScriptCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			Encoding encoding = Encoding.GetEncoding((string)tags["encoding"]);
			DirectoryInfo baseDirectory = Directory.CreateDirectory(Path.Combine(executionContext.ScriptPath, (string)tags["path"]));
			executionContext.Output.WriteLine("Scripting to {0} (Encoding: {1})...", baseDirectory.FullName, encoding.WebName);
			DatabaseInventory inventory = new DatabaseInventory(executionContext.DatabaseInstance, executionContext.Schema);
			inventory.Populate();
			HashSet<string> filesToDelete = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			if ((bool)tags["delete"]) {
				foreach (FileInfo fileInfo in baseDirectory.GetFiles("*.sql", SearchOption.AllDirectories)) {
					filesToDelete.Add(fileInfo.FullName);
				}
			}
			bool objectDirectories = (bool)tags["directories"];
			foreach (CreateStatement statement in inventory.Objects.Where(statement => !(statement is CreateIndexStatement))) {
				statement.ObjectSchema = (string)tags["schema"];
				try {
					string categoryName = string.Empty;
					if (objectDirectories) {
						categoryName = statement.ObjectCategory+"s\\";
					}
					string fileRelativePath = string.Format("{0}{1}.sql", categoryName, statement.ObjectName);
					executionContext.Output.WriteLine("* Scripting {0}", fileRelativePath);
					FileInfo scriptFileName = new FileInfo(Path.Combine(baseDirectory.FullName, fileRelativePath));
					filesToDelete.Remove(scriptFileName.FullName);
					if (scriptFileName.Exists) {
						scriptFileName.Attributes &= ~FileAttributes.ReadOnly;
					} else {
						Directory.CreateDirectory(scriptFileName.DirectoryName);
					}
					using (FileStream stream = scriptFileName.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.Read)) {
						using (StreamWriter writer = new StreamWriter(stream, encoding)) {
							SqlWriter sqlWriter = new SqlWriter(writer);
							statement.WriteTo(sqlWriter);
							CreateTableStatement createTableStatement = statement as CreateTableStatement;
							if (createTableStatement != null) {
								foreach (CreateIndexStatement createIndexStatement in inventory.Objects.OfType<CreateIndexStatement>().Where(s => s.TableName.Name.Equals(createTableStatement.TableName.Name)).OrderBy(s => s.IndexName.Value)) {
									writer.WriteLine(";");
									writer.WriteLine();
									createIndexStatement.WriteTo(sqlWriter);
								}
							}
						}
					}
				} finally {
					statement.ObjectSchema = null;
				}
			}
			foreach (FileInfo deleteFileName in filesToDelete.Select(f => new FileInfo(f))) {
				if (deleteFileName.Exists) {
					executionContext.Output.Write("- Deleting {0}...", deleteFileName.FullName.Substring(baseDirectory.FullName.Length+1));
					try {
						deleteFileName.Attributes &= ~FileAttributes.ReadOnly;
						deleteFileName.Delete();
					} catch (SystemException ex) {
						executionContext.Output.Write(" --> failed: {0}", ex.Message);
					}
					executionContext.Output.WriteLine();
				}
			}
		}

		public override IEnumerable<ITagItem<ExecutionContext>> GetCommandTags() {
			yield return new Tag<ExecutionContext, string>("path", "The target path for the scripts.").SetDefault(context => context.ScriptPath).SetOptional(context => false);
			yield return new Tag<ExecutionContext, bool>("directories", "If true, each object type is put in a separate directory.").SetDefault(context => false);
			yield return new Tag<ExecutionContext, bool>("delete", "If true, all script files not refreshed by this scripting operation deleted.").SetDefault(context => false);
			yield return new Tag<ExecutionContext, string>("schema", "The schema to be used in the scripts.").SetDefault(context => string.Empty).SetOptional(context => true);
			yield return new Tag<ExecutionContext, string>("encoding", "The encoding used to save the scripts.").SetDefault(context => "UTF-8").SetOptional(context => true);
		}
	}
}

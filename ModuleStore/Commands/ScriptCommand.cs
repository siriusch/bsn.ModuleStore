// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Mapper;
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
			DatabaseInventory inventory;
			using (ManagementConnectionProvider provider = new ManagementConnectionProvider(executionContext.Connection, executionContext.Schema)) {
				inventory = new DatabaseInventory(provider, executionContext.Schema);
			}
			HashSet<string> filesToDelete = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			if ((bool)tags["delete"]) {
				foreach (FileInfo fileInfo in baseDirectory.GetFiles("*.sql", SearchOption.AllDirectories)) {
					filesToDelete.Add(fileInfo.FullName);
				}
			}
			bool objectDirectories = (bool)tags["directories"];
			inventory.SetQualification(string.IsNullOrEmpty((string)tags["schema"]) ? "dbo" : (string)tags["schema"]);
			try {
				foreach (CreateStatement statement in inventory.Objects.Where(statement => !(statement is CreateIndexStatement))) {
					string categoryName = string.Empty;
					if (objectDirectories) {
						categoryName = statement.ObjectCategory+"s\\";
					}
					string objectName = statement.ObjectName;
					string fileRelativePath = string.Format("{0}{1}.sql", categoryName, objectName);
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
							sqlWriter.WriteLine(";");
							foreach (CreateIndexStatement createIndexStatement in inventory.Objects.OfType<CreateIndexStatement>().Where(s => s.TableName.Name.Value.Equals(objectName, StringComparison.OrdinalIgnoreCase)).OrderBy(s => s.IndexName.Value)) {
								writer.WriteLine();
								createIndexStatement.WriteTo(sqlWriter);
								sqlWriter.WriteLine(";");
							}
						}
					}
				}
			} finally {
				inventory.UnsetQualification();
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Console.Entities;
using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Console.Commands {
	internal abstract class PerfomingCommandBase: CommandBase<ExecutionContext> {
		protected PerfomingCommandBase(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		protected void ExecuteInternal(ExecutionContext executionContext, IDictionary<string, object> tags, IEnumerable<string> sqlStatements) {
			if ((bool)tags["script"]) {
				foreach (string sql in sqlStatements) {
					executionContext.Output.Write(sql);
					executionContext.Output.WriteLine(";");
					executionContext.Output.WriteLine("GO");
					executionContext.Output.WriteLine();
				}
			} else {
				using (SqlConnection connection = new SqlConnection(executionContext.GetConnectionString())) {
					connection.Open();
					using (SqlTransaction transaction = connection.BeginTransaction()) {
						foreach (string sql in sqlStatements) {
							using (SqlCommand command = connection.CreateCommand()) {
								command.Transaction = transaction;
								command.CommandType = CommandType.Text;
								command.CommandText = sql;
								command.ExecuteNonQuery();
							}
						}
						transaction.Commit();
					}
				}
			}
		}

		public override IEnumerable<ITagItem<ExecutionContext>> GetCommandTags() {
			yield return new Tag<ExecutionContext, bool>("script", "Script only, or perform install on current database").SetDefault(context => true);
		}
	}

	[NamedItem("install", "Generate or execute SQL statements to install a source")]
	internal class InstallCommand: PerfomingCommandBase {
		public InstallCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			Source inventorySource = (Source)tags["source"];
			InstallableInventory inventory = executionContext.GetInventory(inventorySource) as InstallableInventory;
			if (inventory == null) {
				throw new NotSupportedException("The selected inventory type cannot be used as installation source");
			}
			IEnumerable<string> sqlStatements = inventory.GenerateInstallSql(executionContext.Schema);
			ExecuteInternal(executionContext, tags, sqlStatements);
		}

		public override IEnumerable<ITagItem<ExecutionContext>> GetCommandTags() {
			Tag<ExecutionContext, Source> sourceTag = new Tag<ExecutionContext, Source>("source", "Source scripts to use for the installation").SetDefault(context => context.Assembly != null ? Source.Assembly : Source.Files);
			return Merge(base.GetCommandTags(), sourceTag);
		}
	}
}

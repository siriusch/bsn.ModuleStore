using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using bsn.CommandLine.Context;

namespace bsn.ModuleStore.Console.Commands {
	internal abstract class PerfomingCommandBase: CommandBase<ExecutionContext> {
		protected PerfomingCommandBase(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override IEnumerable<ITagItem<ExecutionContext>> GetCommandTags() {
			yield return new Tag<ExecutionContext, bool>("script", "Script only, or perform install on current database").SetDefault(context => true);
		}

		protected void ExecuteInternal(ExecutionContext executionContext, IDictionary<string, object> tags, IEnumerable<string> sqlStatements) {
			if ((bool)tags["script"]) {
				foreach (var sql in sqlStatements) {
					executionContext.Output.Write(sql);
					executionContext.Output.WriteLine(";");
					executionContext.Output.WriteLine("GO");
					executionContext.Output.WriteLine();
				}
			} else {
				using (var connection = new SqlConnection(executionContext.GetConnectionString())) {
					connection.Open();
					using (var transaction = connection.BeginTransaction()) {
						foreach (var sql in sqlStatements) {
							using (var command = connection.CreateCommand()) {
								command.Transaction = transaction;
								command.CommandType = CommandType.Text;
								command.CommandText = sql+';';
								try {
									command.ExecuteNonQuery();
								} catch (Exception) {
									executionContext.Output.WriteLine(sql);
									throw;
								}
							}
						}
						transaction.Commit();
					}
				}
			}
		}
	}
}
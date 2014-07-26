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
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
using System.Data;
using System.Data.SqlClient;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Console.Entities;
using bsn.ModuleStore.Sql;

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
								command.CommandText = sql;
								command.ExecuteNonQuery();
							}
						}
						transaction.Commit();
					}
				}
			}
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
			IEnumerable<string> sqlStatements = inventory.GenerateInstallSql(DatabaseEngine.Unknown, executionContext.Schema);
			ExecuteInternal(executionContext, tags, sqlStatements);
		}

		public override IEnumerable<ITagItem<ExecutionContext>> GetCommandTags() {
			Tag<ExecutionContext, Source> sourceTag = new Tag<ExecutionContext, Source>("source", "Source scripts to use for the installation").SetDefault(context => context.Assembly != null ? Source.Assembly : Source.Files);
			return Merge(base.GetCommandTags(), sourceTag);
		}
	}
}

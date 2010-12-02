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

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Mapper;

namespace bsn.ModuleStore.Console.Configurations {
	[NamedItem("server", "Manage the server and database names.")]
	internal class ServerConfiguration: ConfigurationBase<ExecutionContext>, IConfigurationRead<ExecutionContext>, IConfigurationWrite<ExecutionContext> {
		public IEnumerable<ITagItem<ExecutionContext>> GetReadParameters() {
			yield break;
		}

		public void ShowConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			if (executionContext.Connected) {
				executionContext.Output.WriteLine("Server: {0}", executionContext.Server);
				executionContext.Output.WriteLine("Database: {0}", executionContext.Database);
				string connectionString = executionContext.GetConnectionString();
				executionContext.Output.WriteLine("Connection string: {0}", connectionString);
				executionContext.Output.WriteLine("Database Type: {0}", ModuleDatabase.GetDatabaseType(executionContext.Connection));
				using (ManagementConnectionProvider provider = new ManagementConnectionProvider(executionContext.Connection, "dbo")) {
					executionContext.Output.WriteLine("Database Engine: {0}", provider.Engine);
					executionContext.Output.WriteLine("Database Edition: {0}", provider.EngineEdition);
					executionContext.Output.WriteLine("Database Version: {0}", provider.EngineVersion);
					executionContext.Output.WriteLine("Database Level: {0}", provider.EngineLevel);
				}
			} else {
				executionContext.Output.WriteLine("Not Connected");
			}
		}

		public IEnumerable<ITagItem<ExecutionContext>> GetWriteParameters() {
			yield return new Tag<ExecutionContext, string>("server", "Name of the SQL Server.").SetDefault(context => context.Server);
			yield return new Tag<ExecutionContext, string>("database", "Name of the database on the Server.").SetDefault(context => context.Database).SetOptional(context => !string.IsNullOrEmpty(context.Database));
		}

		public void SetConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			executionContext.Server = (string)parameters["server"];
			executionContext.Database = (string)parameters["database"];
		}
	}
}

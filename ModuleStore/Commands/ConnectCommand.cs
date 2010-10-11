﻿// bsn ModuleStore database versioning
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

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("connect", "Establish the connection to the database server.")]
	internal class ConnectCommand: CommandBase<ExecutionContext> {
		public ConnectCommand(ContextBase<ExecutionContext> owner): base(owner) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			executionContext.Disconnect();
			object value;
			if (tags.TryGetValue("server", out value)) {
				executionContext.Server = (string)value;
			}
			if (tags.TryGetValue("database", out value)) {
				executionContext.Database = (string)value;
			}
			executionContext.Connect();
			executionContext.Output.WriteLine("Connected to database {1} on server {0}", executionContext.Server, executionContext.Database);
		}

		public override IEnumerable<ITagItem<ExecutionContext>> GetCommandTags() {
			yield return new Tag<ExecutionContext, string>("server", "The server to connect to.").SetOptional(context => context.Connected);
			yield return new Tag<ExecutionContext, string>("database", "The database on the server.").SetOptional(context => context.DatabaseInstance != null);
		}
	}
}

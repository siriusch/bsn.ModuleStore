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

using System;
using System.Collections.Generic;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Console.Entities;
using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("update", "Generate or execute SQL statements to update an assembly source")]
	internal class UpdateCommand: PerfomingCommandBase {
		public UpdateCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			AssemblyInventory assemblyInventory = (AssemblyInventory)executionContext.GetInventory(Source.Assembly, false);
			if (assemblyInventory == null) {
				throw new NotSupportedException("The assembly inventory could not be created");
			}
			DatabaseInventory databaseInventory = (DatabaseInventory)executionContext.GetInventory(Source.Database, false);
			if (databaseInventory == null) {
				throw new NotSupportedException("The database inventory could not be created");
			}
#warning perform lookup on ModuleStore tables and perform only updates as necessary on all matching schemas
			base.ExecuteInternal(executionContext, tags, assemblyInventory.GenerateUpdateSql(databaseInventory, 0));
		}
	}
}

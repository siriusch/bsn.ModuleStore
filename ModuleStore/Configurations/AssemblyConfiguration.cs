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
using System.Linq;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Console.Configurations {
	[NamedItem("assembly", "Show information about the loaded assembly.")]
	internal class AssemblyConfiguration: ConfigurationBase<ExecutionContext>, IConfigurationRead<ExecutionContext> {
		public IEnumerable<ITagItem<ExecutionContext>> GetReadParameters() {
			yield break;
		}

		public void ShowConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			if (executionContext.Assembly == null) {
				executionContext.Output.WriteLine(" Failed: No assembly loaded.");
				return;
			}
			executionContext.Output.WriteLine(" Assembly: {0}", executionContext.Assembly);
			AssemblyInventory inventory = new AssemblyInventory(executionContext.Assembly);
			executionContext.Output.WriteLine(" Setup objects: {0}", inventory.Objects.Count);
			executionContext.Output.WriteLine(" Setup statements: {0}", inventory.AdditionalSetupStatements.Count());
			executionContext.Output.WriteLine(" Update scripts: {0}", inventory.UpdateStatements.Count);
		}
	}
}

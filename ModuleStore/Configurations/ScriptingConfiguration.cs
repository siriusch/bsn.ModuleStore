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

namespace bsn.ModuleStore.Console.Configurations {
	[NamedItem("scripting", "Path for scripting and comparing scripts")]
	internal class ScriptingConfiguration: ConfigurationBase<ExecutionContext>, IConfigurationRead<ExecutionContext>, IConfigurationWrite<ExecutionContext> {
		public IEnumerable<ITagItem<ExecutionContext>> GetReadParameters() {
			yield break;
		}

		public void ShowConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			executionContext.Output.WriteLine("Path: {0}", executionContext.ScriptPath);
		}

		public IEnumerable<ITagItem<ExecutionContext>> GetWriteParameters() {
			yield return new Tag<ExecutionContext, string>("path", "The current scripting path.").SetDefault(context => context.ScriptPath);
		}

		public void SetConfiguration(ExecutionContext executionContext, IDictionary<string, object> parameters) {
			executionContext.ScriptPath = (string)parameters["path"];
		}
	}
}

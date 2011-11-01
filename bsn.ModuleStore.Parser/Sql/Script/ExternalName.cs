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
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExternalName: SqlScriptableToken {
		private readonly SqlAssemblyName assemblyName;
		private readonly ClassName className;
		private readonly MethodName methodName;

		[Rule("<ExternalName> ::= ~EXTERNAL ~NAME <AssemblyName> ~'.' <ClassName> ~'.' <MethodName>")]
		public ExternalName(SqlAssemblyName assemblyName, ClassName className, MethodName methodName) {
			Debug.Assert(assemblyName != null);
			Debug.Assert(className != null);
			Debug.Assert(methodName != null);
			this.assemblyName = assemblyName;
			this.className = className;
			this.methodName = methodName;
		}

		public SqlAssemblyName AssemblyName {
			get {
				return assemblyName;
			}
		}

		public ClassName ClassName {
			get {
				return className;
			}
		}

		public MethodName MethodName {
			get {
				return methodName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("EXTERNAL NAME ");
			writer.WriteScript(assemblyName, WhitespacePadding.None);
			writer.Write('.');
			writer.WriteScript(className, WhitespacePadding.None);
			writer.Write('.');
			writer.WriteScript(methodName, WhitespacePadding.None);
		}
	}
}

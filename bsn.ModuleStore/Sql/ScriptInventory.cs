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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public class ScriptInventory: InstallableInventory {
		private readonly string scriptPath;

		public ScriptInventory(string scriptPath, bool deepSearch) {
			this.scriptPath = Directory.GetCurrentDirectory();
			if (!string.IsNullOrEmpty(scriptPath)) {
				this.scriptPath = Path.Combine(this.scriptPath, scriptPath);
			}
			var unsupportedStatements = new List<Statement>();
			foreach (var fileName in Directory.GetFiles(this.scriptPath, "*.sql", deepSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)) {
				unsupportedStatements.Clear();
				using (TextReader reader = new StreamReader(fileName, true)) {
					try {
						ProcessSingleScript(reader, unsupportedStatements.Add);
					} catch (ParseException ex) {
						ex.FileName = fileName;
						throw;
					}
				}
				if (unsupportedStatements.Count > 0) {
					// only files which have no DDL as "unsupported statements" are assumed to be setup scripts
					if (unsupportedStatements.TrueForAll(statement => !(statement is DdlStatement))) {
						foreach (var statement in unsupportedStatements) {
							AddAdditionalSetupStatement(statement);
						}
					} else {
						Trace.WriteLine($"Script {fileName} contains {unsupportedStatements.Count} unsupported statements");
					}
				}
			}
			AdditionalSetupStatementSetSchemaOverride();
		}

		public string ScriptPath => scriptPath;
	}
}

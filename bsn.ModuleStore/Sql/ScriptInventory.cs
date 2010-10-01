using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public class ScriptInventory: InstallableInventory {
		private readonly string scriptPath;

		public ScriptInventory(string scriptPath) {
			this.scriptPath = Directory.GetCurrentDirectory();
			if (!string.IsNullOrEmpty(scriptPath)) {
				this.scriptPath = Path.Combine(this.scriptPath, scriptPath);
			}
			List<Statement> unsupportedStatements = new List<Statement>();
			foreach (string fileName in Directory.GetFiles(this.scriptPath, "*.sql", SearchOption.AllDirectories)) {
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
					// only files which have insert statements only as "unsupported statements" are assumed to be setup scripts
					if (unsupportedStatements.TrueForAll(statement => statement is InsertStatement)) {
						foreach (Statement statement in unsupportedStatements) {
							AddAdditionalSetupStatement(statement);
						}
					} else {
						Debug.WriteLine(string.Format("Script {0} contains {1} unsupported statements", fileName, unsupportedStatements.Count));
					}
				}
			}
			AdditionalSetupStatementSetSchemaOverride();
		}

		public string ScriptPath {
			get {
				return scriptPath;
			}
		}
	}
}
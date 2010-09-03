using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public class ScriptInventory: Inventory {
		private readonly string scriptPath;

		public ScriptInventory(string scriptPath) {
			this.scriptPath = Directory.GetCurrentDirectory();
			if (!string.IsNullOrEmpty(scriptPath)) {
				this.scriptPath = Path.Combine(this.scriptPath, scriptPath);
			}
		}

		public override void Populate() {
			base.Populate();
			List<Statement> unsupportedStatements = new List<Statement>();
			foreach (string fileName in Directory.GetFiles(scriptPath, "*.sql", SearchOption.AllDirectories)) {
				unsupportedStatements.Clear();
				using (TextReader reader = new StreamReader(fileName, true)) {
					ProcessSingleScript(reader, unsupportedStatements.Add);
				}
				Debug.WriteLine(string.Format("Script {0} contains {1} unsupported statements", fileName, unsupportedStatements.Count));
			}
		}
	}
}
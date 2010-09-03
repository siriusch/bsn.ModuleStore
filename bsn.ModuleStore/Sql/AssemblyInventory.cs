using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public class AssemblyInventory: Inventory {
		private readonly AssemblyHandle assembly;
		private readonly ReadOnlyCollection<SqlAssemblyAttribute> attributes;
		private readonly List<Statement> additionalSetupStatements = new List<Statement>();
		private readonly SortedList<int, Statement[]> updateStatements = new SortedList<int, Statement[]>();

		public AssemblyInventory(Assembly assembly): this(new AssemblyHandle(assembly)) {}

		public AssemblyInventory(AssemblyHandle assembly) {
			this.assembly = assembly;
			this.attributes = assembly.GetCustomAttributes(typeof(SqlAssemblyAttribute), true).Cast<SqlAssemblyAttribute>().ToList().AsReadOnly();
		}

		public AssemblyName AssemblyName {
			get {
				return assembly.AssemblyName;
			}
		}

		public ReadOnlyCollection<SqlAssemblyAttribute> Attributes {
			get {
				return attributes;
			}
		}

		public ICollection<Statement> AdditionalSetupStatements {
			get {
				return additionalSetupStatements;
			}
		}

		public SortedList<int, Statement[]> UpdateStatements {
			get {
				return updateStatements;
			}
		}

		private Stream OpenStream(SqlManifestResourceAttribute attribute) {
			Stream result = assembly.GetManifestResourceStream(attribute.ManifestResourceName);
			if (result == null) {
				throw new FileNotFoundException("The embedded SQL file was not found", attribute.ManifestResourceName);
			}
			return result;
		}

		private TextReader OpenText(SqlManifestResourceAttribute attribute) {
			return new StreamReader(OpenStream(attribute), true);
		}

		public override void Populate() {
			base.Populate();
			foreach (SqlAssemblyAttribute attribute in attributes) {
				SqlSetupScriptAttribute setupScriptAttribute = attribute as SqlSetupScriptAttribute;
				if (setupScriptAttribute != null) {
					using (TextReader reader = OpenText(setupScriptAttribute)) {
						ProcessSingleScript(reader, statement => additionalSetupStatements.Add(statement));
					}
				} else {
					SqlUpdateScriptAttribute updateScriptAttribute = attribute as SqlUpdateScriptAttribute;
					if (updateScriptAttribute != null) {
						using (TextReader reader = OpenText(setupScriptAttribute)) {
							updateStatements.Add(updateScriptAttribute.Version, ScriptParser.Parse(reader).ToArray());
						}
					} else {
						Debug.WriteLine(attribute.GetType(), "Unrecognized assembly SQL attribute");
					}
				}
			}
		}
	}
}
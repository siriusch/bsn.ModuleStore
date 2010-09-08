using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public class AssemblyInventory: InstallableInventory {
		private readonly IAssemblyHandle assembly;
		private readonly ReadOnlyCollection<KeyValuePair<SqlAssemblyAttribute, string>> attributes;
		private readonly SortedList<int, Statement[]> updateStatements = new SortedList<int, Statement[]>();

		public AssemblyInventory(Assembly assembly): this(new AssemblyHandle(assembly)) {}

		public AssemblyInventory(IAssemblyHandle assembly) {
			this.assembly = assembly;
			this.attributes = assembly.GetCustomAttributes<SqlAssemblyAttribute>().ToList().AsReadOnly();
			foreach (KeyValuePair<SqlAssemblyAttribute, string> attribute in attributes) {
				SqlSetupScriptAttributeBase setupScriptAttribute = attribute.Key as SqlSetupScriptAttributeBase;
				if (setupScriptAttribute != null) {
					using (TextReader reader = OpenText(setupScriptAttribute, attribute.Value)) {
						ProcessSingleScript(reader, AddAdditionalSetupStatement);
					}
				}
				else {
					SqlUpdateScriptAttribute updateScriptAttribute = attribute.Key as SqlUpdateScriptAttribute;
					if (updateScriptAttribute != null) {
						using (TextReader reader = OpenText(setupScriptAttribute, attribute.Value)) {
							updateStatements.Add(updateScriptAttribute.Version, ScriptParser.Parse(reader).ToArray());
						}
					}
					else {
						Debug.WriteLine(attribute.Key.GetType(), "Unrecognized assembly SQL attribute");
					}
				}
			}
			AdditionalSetupStatementSchemaFixup();
		}

		public AssemblyName AssemblyName {
			get {
				return assembly.AssemblyName;
			}
		}

		public ReadOnlyCollection<KeyValuePair<SqlAssemblyAttribute, string>> Attributes {
			get {
				return attributes;
			}
		}

		public SortedList<int, Statement[]> UpdateStatements {
			get {
				return updateStatements;
			}
		}

		private Stream OpenStream(SqlManifestResourceAttribute attribute, string optionalPrefix) {
			Stream result = assembly.GetManifestResourceStream(attribute.ManifestResourceType, attribute.ManifestResourceName);
			if ((result == null) && (attribute.ManifestResourceType == null) && (!string.IsNullOrEmpty(optionalPrefix))) {
				result = assembly.GetManifestResourceStream(null, optionalPrefix+Type.Delimiter+attribute.ManifestResourceName);
				if (result == null) {
					throw new FileNotFoundException("The embedded SQL file was not found", attribute.ManifestResourceName);
				}
			}
			return result;
		}

		private TextReader OpenText(SqlManifestResourceAttribute attribute, string optionalPrefix) {
			return new StreamReader(OpenStream(attribute, optionalPrefix), true);
		}
	}
}
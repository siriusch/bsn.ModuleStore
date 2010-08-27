using System;
using System.Collections.Generic;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class CreateStatement: DdlStatement {
		private readonly List<IQualifiedName<SchemaName>> schemaQualifiedNames = new List<IQualifiedName<SchemaName>>();

		public abstract ObjectCategory ObjectCategory {
			get;
		}

		public abstract string ObjectName {
			get;
		}

		public string ObjectSchema {
			get {
				return GetObjectSchema() ?? string.Empty;
			}
			set {
				InitializeSchemaNames();
				SchemaName schemaName = string.IsNullOrEmpty(value) ? null : new SchemaName(ObjectName);
				foreach (IQualifiedName<SchemaName> name in schemaQualifiedNames) {
					name.Qualification = schemaName;
				}
			}
		}

		protected abstract string GetObjectSchema();

		public List<IQualifiedName<SchemaName>> SchemaQualifiedNames {
			get {
				return schemaQualifiedNames;
			}
		}

		public override int GetHashCode() {
			InitializeSchemaNames();
			return base.GetHashCode();
		}

		private void InitializeSchemaNames() {
			if (schemaQualifiedNames.Count == 0) {
				string schemaName = GetObjectSchema();
				schemaQualifiedNames.AddRange(this.GetInnerTokens().OfType<IQualifiedName<SchemaName>>().Where(name => name.IsQualified && name.Qualification.Value.Equals(schemaName, StringComparison.OrdinalIgnoreCase)));
			}
		}
	}
}
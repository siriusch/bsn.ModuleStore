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
				SchemaName schemaName = string.IsNullOrEmpty(value) ? null : new SchemaName(value);
				foreach (IQualifiedName<SchemaName> name in SchemaQualifiedNames) {
					name.Qualification = schemaName;
				}
			}
		}

		public IEnumerable<IQualifiedName<SchemaName>> SchemaQualifiedNames {
			get {
				InitializeSchemaNames();
				return schemaQualifiedNames;
			}
		}

		private void InitializeSchemaNames() {
			if (schemaQualifiedNames.Count == 0) {
				string schemaName = GetObjectSchema();
				schemaQualifiedNames.AddRange(this.GetInnerTokens().OfType<IQualifiedName<SchemaName>>().Where(n => n.IsQualified && n.Qualification.Value.Equals(schemaName, StringComparison.OrdinalIgnoreCase)));
			}
		}

		public override int GetHashCode() {
			InitializeSchemaNames();
			return base.GetHashCode();
		}

		protected abstract string GetObjectSchema();

		public IEnumerable<SqlName> GetReferencedObjectNames() {
			return schemaQualifiedNames.Select(qn => qn.Name).Where(n => !n.Value.Equals(ObjectName, StringComparison.OrdinalIgnoreCase)).Distinct();
		}

		internal void ResetSchemaNames() {
			schemaQualifiedNames.Clear();
		}
	}
}
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
		}

		public virtual Statement CreateAlterStatement() {
			return new StatementBlock(CreateDropStatement(), this);
		}

		public abstract DropStatement CreateDropStatement();

		public override int GetHashCode() {
			GetObjectSchemaQualifiedNames();
			return base.GetHashCode();
		}

		public IEnumerable<SqlName> GetReferencedObjectNames() {
			return GetObjectSchemaQualifiedNames().Select(qn => qn.Name).Where(n => !n.Value.Equals(ObjectName, StringComparison.OrdinalIgnoreCase)).Distinct();
		}

		protected abstract string GetObjectSchema();

		internal IEnumerable<IQualifiedName<SchemaName>> GetObjectSchemaQualifiedNames() {
			if (schemaQualifiedNames.Count == 0) {
				string schemaName = GetObjectSchema();
				schemaQualifiedNames.AddRange(GetInnerSchemaQualifiedNames(n => n.Equals(schemaName, StringComparison.OrdinalIgnoreCase)));
			}
			return schemaQualifiedNames;
		}
	}
}

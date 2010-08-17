// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.IO;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public abstract class Inventory {
		private readonly HashSet<CreateStatement> objects = new HashSet<CreateStatement>();
		private readonly List<IQualifiedName<SchemaName>> names = new List<IQualifiedName<SchemaName>>();

		public abstract bool SchemaExists {
			get;
		}

		protected void AddObject(CreateStatement createStatement) {
			createStatement.ResetHash();
			createStatement.GetHash();
			objects.Add(createStatement);
		}

		protected void AddSchemaQualifiedName(IQualifiedName<SchemaName> schemaQualifiedName) {
			if (schemaQualifiedName == null) {
				throw new ArgumentNullException("schemaQualifiedName");
			}
			schemaQualifiedName.Qualification = null;
			schemaQualifiedName.ResetHash();
			schemaQualifiedName.GetHash();
			names.Add(schemaQualifiedName);
		}

		public virtual void Populate() {
			objects.Clear();
			names.Clear();
		}

		public void Dump(string schemaName, TextWriter writer) {
			if (string.IsNullOrEmpty(schemaName)) {
				throw new ArgumentNullException("schemaName");
			}
			SchemaName qualification = new SchemaName(schemaName); 
			SetSchemaName(qualification);
			try {
				SqlWriter sqlWriter = new SqlWriter(writer);
				foreach (CreateStatement statement in objects) {
					statement.WriteTo(sqlWriter);
					writer.WriteLine(";");
				}
			} finally {
				SetSchemaName(null);
			}
		}

		private void SetSchemaName(SchemaName qualification) {
			foreach (IQualifiedName<SchemaName> name in names) {
				name.Qualification = qualification;
			}
		}
	}
}
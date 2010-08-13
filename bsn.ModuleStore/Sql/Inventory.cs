// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public abstract class Inventory {
		private readonly Dictionary<string, CreateStatement> objects = new Dictionary<string, CreateStatement>();

		protected Inventory() {
		}

		public virtual void Populate() {
			objects.Clear();
		}

		public abstract bool SchemaExists {
			get;
		}
	}
}
// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Runtime.Serialization;

namespace bsn.ModuleStore.Sql.Definitions {
	/// <summary>
	/// The base class for all SQL object definitions
	/// </summary>
	public abstract class SqlObject: Metadata<SqlObject> {
		protected SqlObject(string name): base(name) {
		}

		protected SqlObject(SerializationInfo info, StreamingContext context): base(info, context) {}

		/// <summary>
		/// Gets the SQL object kind.
		/// </summary>
		/// <value>The kind.</value>
		public abstract SqlObjectKind Kind {
			get;
		}
	}
}
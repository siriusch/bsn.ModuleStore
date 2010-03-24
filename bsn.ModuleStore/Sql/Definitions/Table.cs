// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace bsn.ModuleStore.Sql.Definitions {
	[Serializable]
	public class Table: SqlObject {
		private readonly List<TableColumn> columns;
	
		public Table(string name): base(name) {}
		protected Table(SerializationInfo info, StreamingContext context): base(info, context) {}

		public override SqlObjectKind Kind {
			get {
				return SqlObjectKind.Table;
			}
		}
	}
}
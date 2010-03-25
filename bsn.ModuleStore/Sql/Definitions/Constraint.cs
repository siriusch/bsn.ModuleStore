// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Runtime.Serialization;

namespace bsn.ModuleStore.Sql.Definitions {
	[Serializable]
	public class Constraint: SqlObject {
		public Constraint(string name): base(name) {}
		protected Constraint(SerializationInfo info, StreamingContext context): base(info, context) {}

		public override SqlObjectKind Kind {
			get {
				return SqlObjectKind.Constraint;
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace bsn.ModuleStore.Sql.Definitions {
	[Serializable]
	public class Index: SqlObject {
		public Index(string name): base(name) {}
		protected Index(SerializationInfo info, StreamingContext context): base(info, context) {}

		public override SqlObjectKind Kind {
			get {
				return SqlObjectKind.Index;
			}
		}
	}
}

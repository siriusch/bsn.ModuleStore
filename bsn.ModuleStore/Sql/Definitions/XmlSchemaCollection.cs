// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Runtime.Serialization;

namespace bsn.ModuleStore.Sql.Definitions {
	[Serializable]
	public class XmlSchemaCollection: SqlObject {
		public XmlSchemaCollection(string name): base(name) {}
		protected XmlSchemaCollection(SerializationInfo info, StreamingContext context): base(info, context) {}

		public override SqlObjectKind Kind {
			get {
				return SqlObjectKind.XmlSchemaCollection;
			}
		}
	}
}
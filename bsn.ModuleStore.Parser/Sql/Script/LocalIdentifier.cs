using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("LocalId")]
	public class LocalIdentifier: SqlIdentifier {
		public LocalIdentifier(string id): base(id) {}
	}
}
using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("Id")]
	public sealed class Identifier: SqlIdentifier {
		public Identifier(string id): base(id) {}
	}
}
using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("TempTableId")]
	public class TempTableIdentifier: SqlIdentifier {
		public TempTableIdentifier(string id): base(id) {}
	}
}
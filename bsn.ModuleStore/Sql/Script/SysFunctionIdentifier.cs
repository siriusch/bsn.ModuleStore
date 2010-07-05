using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("SystemFuncId")]
	public class SysFunctionIdentifier: SqlIdentifier {
		public SysFunctionIdentifier(string id): base(id) {}
	}
}
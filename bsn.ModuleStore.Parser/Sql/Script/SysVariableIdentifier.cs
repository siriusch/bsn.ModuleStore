using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("SystemVarId")]
	public class SysVariableIdentifier: SqlIdentifier {
		public SysVariableIdentifier(string id): base(id) {}
	}
}
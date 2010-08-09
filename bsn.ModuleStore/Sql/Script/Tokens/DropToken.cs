using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("DROP")]
	[Terminal("DROP_PERSISTED")]
	public sealed class DropToken: DdlOperationToken {
		public override DdlOperation Operation {
			get {
				return DdlOperation.Drop;
			}
		}
	}
}
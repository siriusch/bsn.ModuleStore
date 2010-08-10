using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("INSERT")]
	public sealed class InsertToken: DmlOperationToken {
		public override DmlOperation Operation {
			get {
				return DmlOperation.Insert;
			}
		}
	}
}
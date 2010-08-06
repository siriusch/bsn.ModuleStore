using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("ADD")]
	[Terminal("ADD_PERSISTED")]
	public class AddToken: DdlOperationToken {
		public override DdlOperation Operation {
			get {
				return DdlOperation.Add;
			}
		}
	}
}
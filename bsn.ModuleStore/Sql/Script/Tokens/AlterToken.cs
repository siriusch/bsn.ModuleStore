using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("ALTER")]
	public class AlterToken: DdlOperationToken {
		public override DdlOperation Operation {
			get {
				return DdlOperation.Alter;
			}
		}
	}
}
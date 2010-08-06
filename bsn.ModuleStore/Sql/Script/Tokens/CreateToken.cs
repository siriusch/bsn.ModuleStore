using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("CREATE")]
	public class CreateToken: DdlOperationToken {
		public override DdlOperation Operation {
			get {
				return DdlOperation.Create;
			}
		}
	}
}
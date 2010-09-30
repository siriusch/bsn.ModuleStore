using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("ADD")]
	public sealed class Add: DdlOperationToken {
		private readonly string originalValue;

		public Add(string originalValue) {
			this.originalValue = originalValue;
		}

		public override DdlOperation Operation {
			get {
				return DdlOperation.Add;
			}
		}
	}
}
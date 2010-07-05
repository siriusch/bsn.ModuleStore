using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("+")]
	[Terminal("-")]
	[Terminal("/")]
	[Terminal("*")]
	[Terminal("=")]
	[Terminal("<>")]
	[Terminal(">")]
	[Terminal(">=")]
	[Terminal("<")]
	[Terminal("<=")]
	[Terminal("OR")]
	[Terminal("AND")]
	public class OperationToken: SqlToken {
		private readonly string operation;

		public OperationToken(string operation) {
			this.operation = operation;
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write(operation);
		}
	}
}
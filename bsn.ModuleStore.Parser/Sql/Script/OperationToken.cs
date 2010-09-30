using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("+")]
	[Terminal("-")]
	[Terminal("/")]
	[Terminal("*")]
	[Terminal("%")]
	[Terminal("=")]
	[Terminal(">")]
	[Terminal("<")]
	[Terminal(">=")]
	[Terminal("<=")]
	[Terminal("<>")]
	[Terminal("!=")]
	[Terminal("!<")]
	[Terminal("!>")]
	public class OperationToken: SqlScriptableToken {
		private readonly string operation;

		public OperationToken(string operation) {
			Debug.Assert(operation != null);
			this.operation = operation;
		}

		public string Operation {
			get {
				return operation;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write(operation);
		}
	}
}
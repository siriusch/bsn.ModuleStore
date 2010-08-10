using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("+")]
	[Terminal("-")]
	[Terminal("/")]
	[Terminal("*")]
	[Terminal("%")]
	[Terminal("=")]
	[Terminal("<>")]
	[Terminal(">")]
	[Terminal(">=")]
	[Terminal("<")]
	[Terminal("<=")]
	[Terminal("OR")]
	[Terminal("AND")]
	public sealed class OperationToken: SqlToken, IScriptable {
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

		public void WriteTo(TextWriter writer) {
			writer.Write(operation);
		}
	}
}
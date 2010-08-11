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
	public class OperationToken: SqlToken, IScriptable {
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

		public virtual void WriteTo(SqlWriter writer) {
			writer.Write(operation);
		}
	}

	[Terminal("OR")]
	[Terminal("AND")]
	public sealed class OperationNameToken: OperationToken {
		public OperationNameToken(string operation): base(operation) {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write(' ');
			base.WriteTo(writer);
			writer.Write(' ');
		}
	}
}
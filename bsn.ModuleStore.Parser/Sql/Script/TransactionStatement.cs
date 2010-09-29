using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class TransactionStatement: Statement {
		private readonly SqlName transactionIdentifier;

		protected TransactionStatement(SqlName transactionIdentifier) {
			this.transactionIdentifier = transactionIdentifier;
		}

		public SqlName TransactionIdentifier {
			get {
				return transactionIdentifier;
			}
		}

		protected abstract string OperationSpecifier {
			get;
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write(OperationSpecifier);
			writer.Write(" TRANSACTION");
			writer.WriteScript(transactionIdentifier, WhitespacePadding.SpaceBefore);
		}
	}
}
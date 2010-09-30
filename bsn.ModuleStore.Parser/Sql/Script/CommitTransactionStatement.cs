using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CommitTransactionStatement: TransactionStatement {
		[Rule("<CommitTransactionStatement> ::= ~COMMIT")]
		[Rule("<CommitTransactionStatement> ::= ~COMMIT ~WORK")]
		[Rule("<CommitTransactionStatement> ::= ~COMMIT ~TRANSACTION")]
		public CommitTransactionStatement(): this(null) {}

		[Rule("<CommitTransactionStatement> ::= ~COMMIT ~TRANSACTION <TransactionIdentifier>")]
		public CommitTransactionStatement(SqlName transactionIdentifier): base(transactionIdentifier) {}

		protected override string OperationSpecifier {
			get {
				return "COMMIT";
			}
		}
	}
}
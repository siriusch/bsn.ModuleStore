using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class RollbackTransactionStatement: TransactionStatement {
		[Rule("<RollbackTransactionStatement> ::= ~ROLLBACK")]
		[Rule("<RollbackTransactionStatement> ::= ~ROLLBACK ~WORK")]
		[Rule("<RollbackTransactionStatement> ::= ~ROLLBACK ~TRANSACTION")]
		public RollbackTransactionStatement(): this(null) {}

		[Rule("<RollbackTransactionStatement> ::= ~ROLLBACK ~TRANSACTION <TransactionIdentifier>")]
		public RollbackTransactionStatement(SqlName transactionIdentifier): base(transactionIdentifier) {}

		protected override string OperationSpecifier {
			get {
				return "ROLLBACK";
			}
		}
	}
}
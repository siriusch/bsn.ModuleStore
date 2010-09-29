using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SaveTransactionStatement: TransactionStatement {
		[Rule("<SaveTransactionStatement> ::= ~SAVE ~TRANSACTION <TransactionIdentifier>")]
		public SaveTransactionStatement(SqlName transactionIdentifier): base(transactionIdentifier) {}

		protected override string OperationSpecifier {
			get {
				return "SAVE";
			}
		}
	}
}
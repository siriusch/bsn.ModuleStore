using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class BeginTransactionStatement: TransactionStatement {
		[Rule("<BeginTransactionStatement> ::= ~BEGIN ~TRANSACTION")]
		public BeginTransactionStatement(): this(null) {}

		[Rule("<BeginTransactionStatement> ::= ~BEGIN ~TRANSACTION <TransactionIdentifier>")]
		public BeginTransactionStatement(SqlName transactionIdentifier): base(transactionIdentifier) {}

		protected override string OperationSpecifier {
			get {
				return "BEGIN";
			}
		}
	}
}
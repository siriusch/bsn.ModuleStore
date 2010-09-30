using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class BeginTransactionWithMarkStatement: BeginTransactionStatement {
		private readonly StringLiteral markName;

		[Rule("<BeginTransactionStatement> ::= ~BEGIN ~TRANSACTION <TransactionIdentifier> ~WITH ~MARK")]
		public BeginTransactionWithMarkStatement(SqlName transactionIdentifier): this(transactionIdentifier, null) {}

		[Rule("<BeginTransactionStatement> ::= ~BEGIN ~TRANSACTION <TransactionIdentifier> ~WITH ~MARK <StringLiteral>")]
		public BeginTransactionWithMarkStatement(SqlName transactionIdentifier, StringLiteral markName): base(transactionIdentifier) {
			this.markName = markName;
		}

		public StringLiteral MarkName {
			get {
				return markName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write(" WITH MARK");
			writer.WriteScript(markName, WhitespacePadding.SpaceBefore);
		}
	}
}
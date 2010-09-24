using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	[CommonTableExpressionScope]
	public abstract class InsertStatement: Statement {
		private readonly DestinationRowset destinationRowset;
		private readonly QueryHint queryHint;
		private readonly QueryOptions queryOptions;
		private readonly TopExpression topExpression;

		protected InsertStatement(QueryOptions queryOptions, TopExpression topExpression, DestinationRowset destinationRowset, QueryHint queryHint) {
			this.queryOptions = queryOptions;
			this.topExpression = topExpression;
			this.destinationRowset = destinationRowset;
			this.queryHint = queryHint;
		}

		public DestinationRowset DestinationRowset {
			get {
				return destinationRowset;
			}
		}

		public QueryHint QueryHint {
			get {
				return queryHint;
			}
		}

		public QueryOptions QueryOptions {
			get {
				return queryOptions;
			}
		}

		public TopExpression TopExpression {
			get {
				return topExpression;
			}
		}

		public override sealed void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteScript(queryOptions, WhitespacePadding.NewlineAfter);
			writer.Write("INSERT ");
			writer.IncreaseIndent();
			writer.WriteScript(topExpression, WhitespacePadding.SpaceAfter);
			writer.WriteLine();
			writer.Write("INTO ");
			writer.WriteScript(destinationRowset, WhitespacePadding.None);
			WriteToInternal(writer);
			writer.WriteScript(QueryHint, WhitespacePadding.NewlineBefore);
			writer.DecreaseIndent();
		}

		protected abstract void WriteToInternal(SqlWriter writer);
	}
}
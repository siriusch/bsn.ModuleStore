using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class InsertStatement: Statement {
		private readonly List<CommonTableExpression> ctes;
		private readonly DestinationRowset destinationRowset;
		private readonly QueryHint queryHint;
		private readonly TopExpression topExpression;

		protected InsertStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, QueryHint queryHint) {
			this.topExpression = topExpression;
			this.destinationRowset = destinationRowset;
			this.queryHint = queryHint;
			this.ctes = ctes.ToList();
		}

		public List<CommonTableExpression> Ctes {
			get {
				return ctes;
			}
		}

		public DestinationRowset DestinationRowset {
			get {
				return destinationRowset;
			}
		}

		public TopExpression TopExpression {
			get {
				return topExpression;
			}
		}

		public QueryHint QueryHint {
			get {
				return queryHint;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteCommonTableExpressions(ctes);
			writer.Write("INSERT ");
			writer.WriteScript(topExpression, null, " ");
			writer.Write("INTO ");
			writer.WriteScript(destinationRowset);
		}
	}
}
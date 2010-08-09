using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class InsertStatement: SqlStatement {
		private readonly TopExpression topExpression;
		private readonly DestinationRowset destinationRowset;
		private readonly List<CommonTableExpression> ctes;

		protected InsertStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset) {
			this.topExpression = topExpression;
			this.destinationRowset = destinationRowset;
			this.ctes = ctes.ToList();
		}

		public TopExpression TopExpression {
			get {
				return topExpression;
			}
		}

		public DestinationRowset DestinationRowset {
			get {
				return destinationRowset;
			}
		}

		public List<CommonTableExpression> Ctes {
			get {
				return ctes;
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
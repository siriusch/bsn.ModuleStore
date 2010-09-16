﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	[CommonTableExpressionScope]
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

		public IEnumerable<CommonTableExpression> Ctes {
			get {
				return ctes;
			}
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

		public TopExpression TopExpression {
			get {
				return topExpression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteCommonTableExpressions(ctes);
			writer.Write("INSERT ");
			writer.WriteScript(topExpression, WhitespacePadding.SpaceAfter);
			writer.Write("INTO ");
			writer.WriteScript(destinationRowset, WhitespacePadding.None);
		}
	}
}
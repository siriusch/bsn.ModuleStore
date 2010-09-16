﻿using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateNull: PredicateNegable {
		private readonly Expression valueExpression;

		[Rule("<PredicateNull> ::= <Expression> ~IS ~NULL")]
		public PredicateNull(Expression valueExpression): this(valueExpression, false) {}

		protected PredicateNull(Expression valueExpression, bool not): base(not) {
			Debug.Assert(valueExpression != null);
			this.valueExpression = valueExpression;
		}

		public Expression ValueExpression {
			get {
				return valueExpression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(valueExpression, WhitespacePadding.None);
			writer.Write(" IS");
			base.WriteTo(writer);
			writer.Write(" NULL");
		}
	}
}
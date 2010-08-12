using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionTuple: Tuple {
		private readonly List<Expression> valueExpressions;

		[Rule("<Tuple> ::= '(' <ExpressionList> ')'", ConstructorParameterMapping = new[] {1})]
		public ExpressionTuple(Sequence<Expression> value): base() {
			Debug.Assert(value != null);
			valueExpressions = value.ToList();
		}

		public List<Expression> ValueExpressions {
			get {
				return valueExpressions;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write('(');
			writer.WriteSequence(valueExpressions, WhitespacePadding.None, ", ");
			writer.Write(')');
		}
	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionTuple: Tuple {
		private readonly List<Expression> valueExpressions;

		[Rule("<Tuple> ::= ~'(' <ExpressionList> ~')'")]
		public ExpressionTuple(Sequence<Expression> value): base() {
			Debug.Assert(value != null);
			valueExpressions = value.ToList();
		}

		public IEnumerable<Expression> ValueExpressions {
			get {
				return valueExpressions;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write('(');
			writer.WriteScriptSequence(valueExpressions, WhitespacePadding.None, ", ");
			writer.Write(')');
		}
	}
}
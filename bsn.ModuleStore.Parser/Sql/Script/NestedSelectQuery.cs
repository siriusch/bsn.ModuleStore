using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class NestedSelectQuery: Tuple {
		private readonly SelectQuery value;

		[Rule("<Tuple> ::= ~'(' <SelectQuery> ~')'")]
		[Rule("<ExpressionParens> ::= ~'(' <SelectQuery> ~')'")]
		public NestedSelectQuery(SelectQuery value): base() {
			Debug.Assert(value != null);
			this.value = value;
		}

		public SelectQuery Value {
			get {
				return value;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("(");
			writer.IncreaseIndent();
			writer.WriteScript(value, WhitespacePadding.NewlineBefore);
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(')');
		}
	}
}
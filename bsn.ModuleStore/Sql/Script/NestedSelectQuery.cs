using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class NestedSelectQuery: Tuple {
		private readonly SelectQuery value;

		[Rule("<Tuple> ::= '(' <SelectQuery> ')'", ConstructorParameterMapping = new[] {1})]
		[Rule("<ExpressionParens> ::= '(' <SelectQuery> ')'", ConstructorParameterMapping = new[] {1})]
		public NestedSelectQuery(SelectQuery value): base() {
			Debug.Assert(value != null);
			this.value = value;
		}

		public SelectQuery Value {
			get {
				return value;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteLine("(");
			writer.WriteScript(value);
			writer.WriteLine();
			writer.Write(')');
		}
	}
}
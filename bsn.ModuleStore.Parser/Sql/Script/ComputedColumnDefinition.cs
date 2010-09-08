using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ComputedColumnDefinition: ColumnDefinition {
		private readonly Expression expression;

		[Rule("<ColumnDefinition> ::= AS <Expression>", ConstructorParameterMapping = new[] {1})]
		public ComputedColumnDefinition(Expression expression): base() {
			Debug.Assert(expression != null);
			this.expression = expression;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("AS ");
			writer.WriteScript(expression, WhitespacePadding.None);
		}
	}
}
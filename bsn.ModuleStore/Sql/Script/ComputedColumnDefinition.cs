using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ComputedColumnDefinition: ColumnDefinition, IScriptable {
		private readonly Expression expression;

		[Rule("<ColumnDefinition> ::= AS <Expression>", ConstructorParameterMapping = new[] {1})]
		public ComputedColumnDefinition(Expression expression): base() {
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
			this.expression = expression;
		}

		public void WriteTo(TextWriter writer) {
			writer.Write("AS ");
			writer.WriteScript(expression);
		}
	}
}
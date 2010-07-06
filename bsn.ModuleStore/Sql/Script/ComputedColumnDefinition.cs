using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ComputedColumnDefinition: ColumnDefinition {
		private readonly Expression expression;

		[Rule("<TableTypeDefinition> ::= <ColumnName> AS <Expression>", ConstructorParameterMapping = new[] {0, 2})]
		public ComputedColumnDefinition(ColumnName columnName, Expression expression): base(columnName) {
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
			this.expression = expression;
		}

		public override void WriteTo(TextWriter writer) {
			ColumnName.WriteTo(writer);
			writer.Write(" AS ");
			expression.WriteTo(writer);
		}
	}
}
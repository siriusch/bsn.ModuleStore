using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExpressionCastFunction: ExpressionFunction {
		private readonly Expression expression;
		private readonly TypeName typeName;

		[Rule("<ExpressionFunction> ::= CAST_ <Expression> AS <TypeName> ')'", ConstructorParameterMapping = new[] {1, 3})]
		public ExpressionCastFunction(Expression expression, TypeName typeName) {
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
			if (typeName == null) {
				throw new ArgumentNullException("typeName");
			}
			this.expression = expression;
			this.typeName = typeName;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}
		public TypeName TypeName {
			get {
				return typeName;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("CAST(");
			writer.WriteScript(expression);
			writer.Write(" AS ");
			writer.WriteScript(typeName);
			writer.Write(")");
		}
	}
}
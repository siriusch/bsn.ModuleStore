using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionCastFunction: ExpressionFunction {
		private readonly Expression expression;
		private readonly TypeName typeName;

		[Rule("<ExpressionFunction> ::= CAST_ <Expression> AS <TypeName> ')'", ConstructorParameterMapping = new[] {1, 3})]
		public ExpressionCastFunction(Expression expression, TypeName typeName) {
			Debug.Assert(expression != null);
			Debug.Assert(typeName != null);
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

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CAST(");
			writer.WriteScript(expression);
			writer.Write(" AS ");
			writer.WriteScript(typeName);
			writer.Write(")");
		}
	}
}
using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionConvertFunction: ExpressionFunction {
		private readonly IntegerLiteral style;
		private readonly TypeName typeName;
		private readonly Expression valueExpression;

		[Rule("<Value> ::= ~CONVERT ~'(' <TypeName> ~',' <Expression> ~')'")]
		public ExpressionConvertFunction(TypeName typeName, Expression valueExpression): this(typeName, valueExpression, null) {}

		[Rule("<Value> ::= ~CONVERT ~'(' <TypeName> ~',' <Expression> ~',' IntegerLiteral ~')'")]
		public ExpressionConvertFunction(TypeName typeName, Expression valueExpression, IntegerLiteral style): base() {
			Debug.Assert(typeName != null);
			Debug.Assert(valueExpression != null);
			this.typeName = typeName;
			this.valueExpression = valueExpression;
			this.style = style;
		}

		public IntegerLiteral Style {
			get {
				return style;
			}
		}

		public TypeName TypeName {
			get {
				return typeName;
			}
		}

		public Expression ValueExpression {
			get {
				return valueExpression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CONVERT(");
			writer.WriteScript(typeName, WhitespacePadding.None);
			writer.Write(", ");
			writer.WriteScript(valueExpression, WhitespacePadding.None);
			writer.WriteScript(style, WhitespacePadding.None, ", ", null);
			writer.Write(')');
		}
	}
}
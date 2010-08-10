using System;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnCheckConstraint: ColumnNamedConstraintBase {
		private readonly Expression expression;
		private readonly bool notForReplication;

		[Rule("<NamedColumnConstraint> ::= CHECK <OptionalNotForReplication> '(' <Expression> ')'", ConstructorParameterMapping = new[] {1, 3})]
		public ColumnCheckConstraint(Optional<ForReplicationToken> notForReplication, Expression expression): this(null, notForReplication, expression) {}

		[Rule("<NamedColumnConstraint> ::= CONSTRAINT <ConstraintName> CHECK <OptionalNotForReplication> '(' <Expression> ')'", ConstructorParameterMapping = new[] {1, 3, 5})]
		public ColumnCheckConstraint(ConstraintName constraintName, Optional<ForReplicationToken> notForReplication, Expression expression): base(constraintName) {
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
			this.expression = expression;
			this.notForReplication = notForReplication.HasValue();
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public bool NotForReplication {
			get {
				return notForReplication;
			}
		}

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			writer.Write("CHECK ");
			writer.WriteNotForReplication(notForReplication, null, " ");
			writer.Write('(');
			writer.WriteScript(expression);
			writer.Write(')');
		}
	}
}
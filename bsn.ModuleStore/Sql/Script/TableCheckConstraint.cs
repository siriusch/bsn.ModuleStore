using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TableCheckConstraint: TableConstraint {
		private readonly Expression expression;
		private readonly bool notForReplication;

		[Rule("<TableConstraint> ::= CHECK <OptionalNotForReplication> '(' <Expression> ')'", ConstructorParameterMapping = new[] {1, 3})]
		public TableCheckConstraint(Optional<ForReplicationToken> notForReplication, Expression expression): this(null, notForReplication, expression) {}

		[Rule("<TableConstraint> ::= CONSTRAINT <ConstraintName> CHECK <OptionalNotForReplication> '(' <Expression> ')'", ConstructorParameterMapping = new[] {1, 3, 5})]
		public TableCheckConstraint(ConstraintName constraintName, Optional<ForReplicationToken> notForReplication, Expression expression): base(constraintName) {
			Debug.Assert(expression != null);
			this.notForReplication = notForReplication.HasValue();
			this.expression = expression;
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
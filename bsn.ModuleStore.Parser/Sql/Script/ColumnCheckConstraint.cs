using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnCheckConstraint: ColumnNamedConstraintBase {
		private readonly Expression expression;
		private readonly bool notForReplication;

		[Rule("<NamedColumnConstraint> ::= ~CHECK <OptionalNotForReplication> ~'(' <Expression> ~')'")]
		public ColumnCheckConstraint(Optional<ForReplicationToken> notForReplication, Expression expression): this(null, notForReplication, expression) {}

		[Rule("<NamedColumnConstraint> ::= ~CONSTRAINT <ConstraintName> ~CHECK <OptionalNotForReplication> ~'(' <Expression> ~')'")]
		public ColumnCheckConstraint(ConstraintName constraintName, Optional<ForReplicationToken> notForReplication, Expression expression): base(constraintName) {
			Debug.Assert(expression != null);
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

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("CHECK ");
			writer.WriteNotForReplication(notForReplication, WhitespacePadding.SpaceAfter);
			writer.Write('(');
			writer.WriteScript(expression, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}
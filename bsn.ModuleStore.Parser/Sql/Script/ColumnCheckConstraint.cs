using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnCheckConstraint: ColumnNamedConstraintBase {
		private readonly Expression expression;
		private readonly ReplicationToken replication;

		[Rule("<NamedColumnConstraint> ::= ~CHECK <OptionalNotForReplication> ~'(' <Expression> ~')'")]
		public ColumnCheckConstraint(ReplicationToken replication, Expression expression): this(null, replication, expression) {}

		[Rule("<NamedColumnConstraint> ::= ~CONSTRAINT <ConstraintName> ~CHECK <OptionalNotForReplication> ~'(' <Expression> ~')'")]
		public ColumnCheckConstraint(ConstraintName constraintName, ReplicationToken replication, Expression expression): base(constraintName) {
			Debug.Assert(expression != null);
			this.replication = replication;
			this.expression = expression;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public ReplicationToken Replication {
			get {
				return replication;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("CHECK ");
			writer.WriteScript(replication, WhitespacePadding.SpaceAfter);
			writer.WriteScript(expression, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}
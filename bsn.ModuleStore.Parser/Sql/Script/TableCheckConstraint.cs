using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TableCheckConstraint: TableConstraint {
		private readonly Expression expression;
		private readonly ReplicationToken replication;

		[Rule("<TableConstraint> ::= ~CHECK <OptionalNotForReplication> ~'(' <Expression> ~')'")]
		public TableCheckConstraint(ReplicationToken replication, Expression expression): this(null, replication, expression) {}

		[Rule("<TableConstraint> ::= ~CONSTRAINT <ConstraintName> ~CHECK <OptionalNotForReplication> ~'(' <Expression> ~')'")]
		public TableCheckConstraint(ConstraintName constraintName, ReplicationToken replication, Expression expression): base(constraintName) {
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
			writer.Write('(');
			writer.WriteScript(expression, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}
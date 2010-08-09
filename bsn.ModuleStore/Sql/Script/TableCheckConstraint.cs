using System;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class TableCheckConstraint: TableConstraint {
		private readonly bool notForReplication;
		private readonly Expression expression;

		[Rule("<TableConstraint> ::= CHECK <OptionalNotForReplication> '(' <Expression> ')'", ConstructorParameterMapping = new[] {1, 3})]
		public TableCheckConstraint(Optional<ForReplicationToken> notForReplication, Expression expression): this(null, notForReplication, expression) {}

		[Rule("<TableConstraint> ::= CONSTRAINT <ConstraintName> CHECK <OptionalNotForReplication> '(' <Expression> ')'", ConstructorParameterMapping = new[] {1, 3, 5})]
		public TableCheckConstraint(ConstraintName constraintName, Optional<ForReplicationToken> notForReplication, Expression expression): base(constraintName) {
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
			this.notForReplication = notForReplication.HasValue();
			this.expression = expression;
		}

		public bool NotForReplication {
			get {
				return notForReplication;
			}
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			writer.Write();
		}
	}
}
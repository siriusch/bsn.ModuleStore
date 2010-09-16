using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script;

[assembly: RuleTrim("<ConstraintDefaultValue> ::= '(' <ConstraintDefaultValue> ')'", "<ConstraintDefaultValue>", SemanticTokenType = typeof(SqlToken))]

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnDefaultConstraint: ColumnNamedConstraintBase {
		private readonly Expression defaultValue;

		[Rule("<NamedColumnConstraint> ::= ~DEFAULT <ConstraintDefaultValue>")]
		public ColumnDefaultConstraint(Expression defaultValue): this(null, defaultValue) {}

		[Rule("<NamedColumnConstraint> ::= ~CONSTRAINT <ConstraintName> ~DEFAULT <ConstraintDefaultValue>")]
		public ColumnDefaultConstraint(ConstraintName constraintName, Expression defaultValue): base(constraintName) {
			Debug.Assert(defaultValue != null);
			this.defaultValue = defaultValue;
		}

		public Expression DefaultValue {
			get {
				return defaultValue;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("DEFAULT (");
			writer.WriteScript(defaultValue, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}
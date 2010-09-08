using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionCountFunction: ExpressionFunction {
		private readonly Qualified<SqlName, ColumnName> columnName;
		private readonly bool? restriction;

		[Rule("<ExpressionCountFunction> ::= COUNT_ <Restriction> <ColumnWildNameQualified> ')'", ConstructorParameterMapping = new[] {1, 2})]
		public ExpressionCountFunction(DuplicateRestrictionToken restriction, Qualified<SqlName, ColumnName> columnName): this(restriction.Distinct, columnName) {}

		[Rule("<ExpressionCountFunction> ::= COUNT_ <ColumnWildNameQualified> ')'", ConstructorParameterMapping = new[] {1})]
		public ExpressionCountFunction(Qualified<SqlName, ColumnName> columnName): this(default(bool?), columnName) {}

		private ExpressionCountFunction(bool? restriction, Qualified<SqlName, ColumnName> columnName) {
			Debug.Assert(columnName != null);
			this.restriction = restriction;
			this.columnName = columnName;
		}

		public Qualified<SqlName, ColumnName> ColumnName {
			get {
				return columnName;
			}
		}

		public bool? Restriction {
			get {
				return restriction;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("COUNT(");
			writer.WriteDuplicateRestriction(restriction, WhitespacePadding.SpaceAfter);
			writer.WriteScript(columnName, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}
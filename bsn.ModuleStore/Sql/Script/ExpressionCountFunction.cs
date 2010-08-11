using System;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionCountFunction: ExpressionFunction {
		private readonly Qualified<ColumnName> columnName;
		private readonly bool? restriction;

		[Rule("<ExpressionFunction> ::= COUNT_ <Restriction> <ColumnWildNameQualified> ')'", ConstructorParameterMapping = new[] {1, 2})]
		public ExpressionCountFunction(DuplicateRestrictionToken restriction, Qualified<ColumnName> columnName): this(restriction.Distinct, columnName) {}

		[Rule("<ExpressionFunction> ::= COUNT_ <ColumnWildNameQualified> ')'", ConstructorParameterMapping=new[] { 1 })]
		public ExpressionCountFunction(Qualified<ColumnName> columnName) : this(default(bool?), columnName) {
		}

		private ExpressionCountFunction(bool? restriction, Qualified<ColumnName> columnName) {
			if (columnName == null) {
				throw new ArgumentNullException("columnName");
			}
			this.restriction = restriction;
			this.columnName = columnName;
		}

		public Qualified<ColumnName> ColumnName {
			get {
				return columnName;
			}
		}

		public bool? Restriction {
			get {
				return restriction;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("COUNT(");
			writer.WriteDuplicateRestriction(restriction, null, " ");
			writer.WriteScript(columnName);
			writer.Write(')');
		}
	}
}
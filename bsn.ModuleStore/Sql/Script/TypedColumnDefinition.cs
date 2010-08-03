using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TypedColumnDefinition: ColumnDefinition {
		private readonly Qualified<TypeName> columnType;
		private readonly Sequence<ColumnConstraint> constraints;

		[Rule("<ColumnDefinition> ::= <TypeNameQualified> <ColumnConstraintList>")]
		public TypedColumnDefinition(Qualified<TypeName> columnType, Sequence<ColumnConstraint> constraints): base() {
			if (columnType == null) {
				throw new ArgumentNullException("columnType");
			}
			if (constraints == null) {
				throw new ArgumentNullException("constraints");
			}
			this.columnType = columnType;
			this.constraints = constraints;
		}

		public override void WriteTo(TextWriter writer) {
			columnType.WriteTo(writer);
			foreach (ColumnConstraint constraint in constraints) {
				writer.Write(' ');
				constraint.WriteTo(writer);
			}
		}
	}
}
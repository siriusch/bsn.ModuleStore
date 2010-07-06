using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ColumnType: SqlToken {
		private readonly Sequence<ColumnConstraint> constraints;
		private readonly Qualified<TypeName> typeName;

		[Rule("<ColumnTypeDefinition> ::= <TypeNameQualified> <ColumnConstraintList>")]
		public ColumnType(Qualified<TypeName> typeName, Sequence<ColumnConstraint> constraints) {
			if (typeName == null) {
				throw new ArgumentNullException("typeName");
			}
			this.typeName = typeName;
			this.constraints = constraints;
		}

		public override void WriteTo(TextWriter writer) {
			typeName.WriteTo(writer);
			foreach (ColumnConstraint constraint in constraints) {
				writer.Write(' ');
				constraint.WriteTo(writer);
			}
		}
	}
}
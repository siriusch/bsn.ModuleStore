using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TypedColumnDefinition: ColumnDefinition {
		private readonly Qualified<TypeName> columnType;
		private readonly List<ColumnConstraint> constraints;

		[Rule("<ColumnDefinition> ::= <TypeNameQualified> <ColumnConstraintList>")]
		public TypedColumnDefinition(Qualified<TypeName> columnType, Sequence<ColumnConstraint> constraints): base() {
			Debug.Assert(columnType != null);
			Debug.Assert(constraints != null);
			this.columnType = columnType;
			this.constraints = constraints.ToList();
		}

		public Qualified<TypeName> ColumnType {
			get {
				return columnType;
			}
		}

		public List<ColumnConstraint> Constraints {
			get {
				return constraints;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(columnType);
			writer.WriteSequence(constraints, " ", null, null);
		}
	}
}
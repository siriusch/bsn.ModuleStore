using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TypedColumnDefinition: ColumnDefinition {
		private readonly Qualified<SchemaName, TypeName> columnType;
		private readonly List<ColumnConstraint> constraints;

		[Rule("<ColumnDefinition> ::= <TypeNameQualified> <ColumnConstraintList>")]
		public TypedColumnDefinition(Qualified<SchemaName, TypeName> columnType, Sequence<ColumnConstraint> constraints): base() {
			Debug.Assert(columnType != null);
			Debug.Assert(constraints != null);
			this.columnType = columnType;
			this.constraints = constraints.ToList();
		}

		public Qualified<SchemaName, TypeName> ColumnType {
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
			writer.WriteScript(columnType, WhitespacePadding.None);
			writer.WriteSequence(constraints, WhitespacePadding.SpaceBefore, null);
		}
	}
}
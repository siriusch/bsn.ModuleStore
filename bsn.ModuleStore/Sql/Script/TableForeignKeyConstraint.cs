using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TableForeignKeyConstraint: TableConstraint {
		private readonly List<ColumnName> columnNames;
		private readonly List<ForeignKeyAction> keyActions;
		private readonly List<ColumnName> refColumnNames;
		private readonly Qualified<SchemaName, TableName> refTableName;

		[Rule("<TableConstraint> ::= FOREIGN KEY '(' <ColumnNameList> ')' REFERENCES <TableNameQualified> <ColumnNameGroup> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {3, 6, 7, 8})]
		public TableForeignKeyConstraint(Sequence<ColumnName> columnNames, Qualified<SchemaName, TableName> refTableName, Optional<Sequence<ColumnName>> refColumnNames, Sequence<ForeignKeyAction> keyActions) : this(null, columnNames, refTableName, refColumnNames, keyActions) {
		}

		[Rule("<TableConstraint> ::= CONSTRAINT <ConstraintName> FOREIGN KEY '(' <ColumnNameList> ')' REFERENCES <TableNameQualified> <ColumnNameGroup> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {1, 5, 8, 9, 10})]
		public TableForeignKeyConstraint(ConstraintName constraintName, Sequence<ColumnName> columnNames, Qualified<SchemaName, TableName> refTableName, Optional<Sequence<ColumnName>> refColumnNames, Sequence<ForeignKeyAction> keyActions)
			: base(constraintName) {
			Debug.Assert(refTableName != null);
			this.columnNames = columnNames.ToList();
			this.refTableName = refTableName;
			this.refColumnNames = refColumnNames.ToList();
			this.keyActions = keyActions.ToList();
		}

		public List<ColumnName> ColumnNames {
			get {
				return columnNames;
			}
		}

		public List<ForeignKeyAction> KeyActions {
			get {
				return keyActions;
			}
		}

		public List<ColumnName> RefColumnNames {
			get {
				return refColumnNames;
			}
		}

		public Qualified<SchemaName, TableName> RefTableName {
			get {
				return refTableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("FOREIGN KEY (");
			writer.WriteScriptSequence(columnNames, WhitespacePadding.None, ", ");
			writer.Write(") REFERENCES ");
			writer.WriteScript(refTableName, WhitespacePadding.None);
			if (refColumnNames.Count > 0) {
				writer.Write(" (");
				writer.WriteScriptSequence(refColumnNames, WhitespacePadding.None, ", ");
				writer.Write(')');
			}
			writer.WriteScriptSequence(keyActions, WhitespacePadding.SpaceBefore, null);
		}
	}
}
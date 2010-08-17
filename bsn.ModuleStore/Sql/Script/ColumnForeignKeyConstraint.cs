using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnForeignKeyConstraint: ColumnNamedConstraintBase {
		private readonly List<ForeignKeyAction> keyActions;
		private readonly ColumnName refColumnName;
		private readonly Qualified<SchemaName, TableName> tableName;

		[Rule("<NamedColumnConstraint> ::= REFERENCES <TableNameQualified> <OptionalForeignRefColumn> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {1, 2, 3})]
		[Rule("<NamedColumnConstraint> ::= FOREIGN KEY REFERENCES <TableNameQualified> <OptionalForeignRefColumn> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {3, 4, 5})]
		public ColumnForeignKeyConstraint(Qualified<SchemaName, TableName> tableName, Optional<ColumnName> refColumnName, Sequence<ForeignKeyAction> keyActions) : this(null, tableName, refColumnName, keyActions) {
		}

		[Rule("<NamedColumnConstraint> ::= CONSTRAINT <ConstraintName> REFERENCES <TableNameQualified> <OptionalForeignRefColumn> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {1, 3, 4, 5})]
		[Rule("<NamedColumnConstraint> ::= CONSTRAINT <ConstraintName> FOREIGN KEY REFERENCES <TableNameQualified> <OptionalForeignRefColumn> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {1, 5, 6, 7})]
		public ColumnForeignKeyConstraint(ConstraintName constraintName, Qualified<SchemaName, TableName> tableName, Optional<ColumnName> refColumnName, Sequence<ForeignKeyAction> keyActions): base(constraintName) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
			this.refColumnName = refColumnName;
			this.keyActions = keyActions.ToList();
		}

		public List<ForeignKeyAction> KeyActions {
			get {
				return keyActions;
			}
		}

		public ColumnName RefColumnName {
			get {
				return refColumnName;
			}
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("REFERENCES ");
			writer.WriteScript(tableName, WhitespacePadding.None);
			writer.WriteScript(refColumnName, WhitespacePadding.SpaceBefore);
			writer.WriteScriptSequence(keyActions, WhitespacePadding.SpaceBefore, null);
		}
	}
}
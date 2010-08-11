using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnForeignKeyConstraint: ColumnNamedConstraintBase {
		private readonly List<ForeignKeyAction> keyActions;
		private readonly ColumnName refColumnName;
		private readonly TableName tableName;

		[Rule("<NamedColumnConstraint> ::= REFERENCES <TableName> <OptionalForeignRefColumn> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {1, 2, 3})]
		[Rule("<NamedColumnConstraint> ::= FOREIGN KEY REFERENCES <TableName> <OptionalForeignRefColumn> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {3, 4, 5})]
		public ColumnForeignKeyConstraint(TableName tableName, Optional<ColumnName> refColumnName, Sequence<ForeignKeyAction> keyActions): this(null, tableName, refColumnName, keyActions) {}

		[Rule("<NamedColumnConstraint> ::= CONSTRAINT <ConstraintName> REFERENCES <TableName> <OptionalForeignRefColumn> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {1, 3, 4, 5})]
		[Rule("<NamedColumnConstraint> ::= CONSTRAINT <ConstraintName> FOREIGN KEY REFERENCES <TableName> <OptionalForeignRefColumn> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {1, 5, 6, 7})]
		public ColumnForeignKeyConstraint(ConstraintName constraintName, TableName tableName, Optional<ColumnName> refColumnName, Sequence<ForeignKeyAction> keyActions): base(constraintName) {
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

		public TableName TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			writer.Write("REFERENCES ");
			writer.WriteScript(tableName);
			writer.WriteScript(refColumnName, " ", null);
			writer.WriteSequence(keyActions, " ", null, null);
		}
	}
}
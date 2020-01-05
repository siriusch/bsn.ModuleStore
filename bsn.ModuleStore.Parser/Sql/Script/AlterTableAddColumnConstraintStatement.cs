using System;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class AlterTableAddColumnConstraintStatement: AlterTableStatement, IApplicableTo<CreateTableStatement> {
		private readonly TableCheck check;
		private readonly ColumnName columnName;
		private readonly ColumnConstraint constraint;

		[Rule("<AlterTableStatement> ::= ~ALTER ~TABLE <TableNameQualified> <TableCheck> ~ADD <ColumnConstraint> ~FOR <ColumnName>")]
		public AlterTableAddColumnConstraintStatement(Qualified<SchemaName, TableName> tableName, TableCheckToken check, ColumnConstraint constraint, ColumnName columnName): base(tableName) {
			Debug.Assert(check != null);
			Debug.Assert(columnName != null);
			Debug.Assert(constraint != null);
			this.check = check.TableCheck;
			this.constraint = constraint;
			this.columnName = columnName;
		}

		public TableCheck Check => check;

		public ColumnName ColumnName => columnName;

		public ColumnConstraint Constraint => constraint;

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteEnum(check, WhitespacePadding.SpaceAfter);
			writer.WriteKeyword("ADD ");
			writer.WriteScript(constraint, WhitespacePadding.None);
			writer.WriteKeyword(" FOR ");
			writer.WriteScript(columnName, WhitespacePadding.None);
		}

		IQualifiedName<SchemaName> IApplicableTo<CreateTableStatement>.QualifiedName => TableName;

		public void ApplyTo(CreateTableStatement instance) {
			var typedColumnDefinition = instance.Definitions.OfType<TableColumnDefinition>().Where(c => string.Equals(c.ColumnName.Value, columnName.Value, StringComparison.OrdinalIgnoreCase)).Select(c => c.ColumnDefinition).OfType<TypedColumnDefinition>().FirstOrDefault();
			if (typedColumnDefinition == null) {
				throw new InvalidOperationException($"The column {columnName} was not found on table {TableName}");
			}
			typedColumnDefinition.Constraints.Add(constraint);
		}
	}
}

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

		public TableCheck Check {
			get {
				return check;
			}
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public ColumnConstraint Constraint {
			get {
				return constraint;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteEnum(check, WhitespacePadding.SpaceAfter);
			writer.Write("ADD ");
			writer.WriteScript(constraint, WhitespacePadding.None);
			writer.Write(" FOR ");
			writer.WriteScript(columnName, WhitespacePadding.None);
		}

		IQualifiedName<SchemaName> IApplicableTo<CreateTableStatement>.QualifiedName {
			get {
				return TableName;
			}
		}

		public void ApplyTo(CreateTableStatement instance) {
			TypedColumnDefinition typedColumnDefinition = instance.Definitions.OfType<TableColumnDefinition>().Where(c => string.Equals(c.ColumnName.Value, columnName.Value, StringComparison.OrdinalIgnoreCase)).Select(c => c.ColumnDefinition).OfType<TypedColumnDefinition>().FirstOrDefault();
			if (typedColumnDefinition == null) {
				throw new InvalidOperationException(string.Format("The column {0} was not found on table {1}", columnName, TableName));
			}
			typedColumnDefinition.Constraints.Add(constraint);
		}
	}
}

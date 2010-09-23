using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExpressionColumnFunctionCalls: ExpressionFunctionCalls {
		private readonly Qualified<TableName, ColumnName> columnNameQualified;

		[Rule("<Value> ::= <TableName> ~'.' <ColumnName> ~'.' <NamedFunctionList>")]
		public ExpressionColumnFunctionCalls(TableName tableName, ColumnName columnName, Sequence<NamedFunction> functions): base(functions.Item, functions.Next) {
			Debug.Assert(tableName != null);
			Debug.Assert(columnName != null);
			columnNameQualified = new Qualified<TableName, ColumnName>(tableName, columnName);
			Debug.Assert(!functions.Item.FunctionName.IsQualified);
			functions.Item.FunctionName.LockOverride();
		}

		public Qualified<TableName, ColumnName> ColumnNameQualified {
			get {
				return columnNameQualified;
			}
		}

		protected override void WriteToInternal(SqlWriter writer) {
			writer.WriteScript(columnNameQualified, WhitespacePadding.None);
			writer.Write('.');
			base.WriteToInternal(writer);
		}
	}
}
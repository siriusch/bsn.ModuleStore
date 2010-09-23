using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionColumnFunction: ExpressionFunction {
		private readonly Qualified<TableName, ColumnName> columnNameQualified;
		private readonly NamedFunction functionCall;

		[Rule("<Value> ::= <TableName> ~'.' <ColumnName> ~'.' <FunctionCall>")]
		public ExpressionColumnFunction(TableName tableName, ColumnName columnName, NamedFunction functionCall) {
			Debug.Assert(!functionCall.FunctionName.IsQualified);
			columnNameQualified = new Qualified<TableName, ColumnName>(tableName, columnName);
			functionCall.FunctionName.LockOverride();
			this.functionCall = functionCall;
		}

		public Qualified<TableName, ColumnName> ColumnNameQualified {
			get {
				return columnNameQualified;
			}
		}

		public NamedFunction FunctionCall {
			get {
				return functionCall;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(columnNameQualified, WhitespacePadding.None);
			writer.Write('.');
			writer.WriteScript(functionCall, WhitespacePadding.None);
		}
	}
}
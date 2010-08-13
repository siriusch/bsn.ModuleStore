using System;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropFunctionStatement: DropStatement {
		private readonly Qualified<SchemaName, FunctionName> functionName;

		[Rule("<DropFunctionStatement> ::= DROP FUNCTION <FunctionNameQualified>", ConstructorParameterMapping = new[] {2})]
		public DropFunctionStatement(Qualified<SchemaName, FunctionName> functionName) {
			Debug.Assert(functionName != null);
			this.functionName = functionName;
		}

		public Qualified<SchemaName, FunctionName> FunctionName {
			get {
				return functionName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("DROP FUNCTION ");
			writer.WriteScript(functionName, WhitespacePadding.None);
		}
	}
}
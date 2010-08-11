using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropFunctionStatement: DropStatement {
		private readonly FunctionName functionName;

		[Rule("<DropFunctionStatement> ::= DROP FUNCTION <FunctionName>", ConstructorParameterMapping = new[] {2})]
		public DropFunctionStatement(FunctionName functionName) {
			Debug.Assert(functionName != null);
			this.functionName = functionName;
		}

		public FunctionName FunctionName {
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
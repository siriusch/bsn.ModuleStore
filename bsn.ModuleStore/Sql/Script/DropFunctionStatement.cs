using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DropFunctionStatement: SqlStatement {
		private readonly FunctionName functionName;

		[Rule("<DropFunctionStatement> ::= DROP FUNCTION <FunctionName>", ConstructorParameterMapping = new[] {2})]
		public DropFunctionStatement(FunctionName functionName) {
			if (functionName == null) {
				throw new ArgumentNullException("functionName");
			}
			this.functionName = functionName;
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("DROP FUNCTION ");
			functionName.WriteTo(writer);
		}
	}
}

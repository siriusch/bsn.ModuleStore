using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script;

[assembly: RuleTrim("<FunctionInlineSelect> ::= '(' <FunctionInlineSelect> ')'", "<FunctionInlineSelect>", SemanticTokenType = typeof(SqlToken))]
[assembly: RuleTrim("<OrderClause> ::= ORDER BY <OrderList>", "<OrderList>", SemanticTokenType = typeof(SqlToken))]

namespace bsn.ModuleStore.Sql.Script {
	public abstract class CreateFunctionStatement<TBody>: SqlCreateStatement where TBody: SqlStatement {
		private readonly TBody body;
		private readonly FunctionName functionName;
		private readonly FunctionOption option;
		private readonly List<FunctionParameter> parameters;

		protected CreateFunctionStatement(FunctionName functionName, Sequence<FunctionParameter> parameters, FunctionOptionToken option, TBody body) {
			if (functionName == null) {
				throw new ArgumentNullException("functionName");
			}
			if (body == null) {
				throw new ArgumentNullException("body");
			}
			this.functionName = functionName;
			this.parameters = parameters.ToList();
			this.option = option.FunctionOption;
			this.body = body;
		}

		public TBody Body {
			get {
				return body;
			}
		}

		public FunctionName FunctionName {
			get {
				return functionName;
			}
		}

		public FunctionOption Option {
			get {
				return option;
			}
		}

		public List<FunctionParameter> Parameters {
			get {
				return parameters;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("CREATE FUNCTION ");
			writer.WriteScript(functionName);
			writer.Write(" (");
			writer.WriteSequence(parameters, null, ", ", null);
			writer.Write(") RETURNS ");
		}
	}
}
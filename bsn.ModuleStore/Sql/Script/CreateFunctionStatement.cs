using System;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script;

[assembly: RuleTrim("<FunctionInlineSelect> ::= '(' <FunctionInlineSelect> ')'", "<FunctionInlineSelect>", SemanticTokenType = typeof(SqlToken))]
[assembly: RuleTrim("<OrderClause> ::= ORDER BY <OrderList>", "<OrderList>", SemanticTokenType=typeof(SqlToken))]

namespace bsn.ModuleStore.Sql.Script {
	public abstract class CreateFunctionStatement<TBody>: SqlCreateStatement where TBody: SqlStatement {
		private readonly TBody body;
		private readonly FunctionName functionName;
		private readonly FunctionOption options;
		private readonly Sequence<FunctionParameter> parameters;

		protected CreateFunctionStatement(FunctionName functionName, Optional<Sequence<FunctionParameter>> parameters, FunctionOption options, TBody body) {
			if (functionName == null) {
				throw new ArgumentNullException("functionName");
			}
			if (body == null) {
				throw new ArgumentNullException("body");
			}
			this.functionName = functionName;
			this.parameters = parameters;
			this.options = options;
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

		public FunctionOption Options {
			get {
				return options;
			}
		}

		public Sequence<FunctionParameter> Parameters {
			get {
				return parameters;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("CREATE FUNCTION ");
			functionName.WriteTo(writer);
			writer.Write(" (");
			if (parameters != null) {
				string separator = string.Empty;
				foreach (FunctionParameter parameter in parameters) {
					writer.Write(separator);
					parameter.WriteTo(writer);
					separator = ", ";
				}
			}
			writer.Write(") RETURNS ");
		}

		protected void WriteOptions(TextWriter writer) {
			if (Options != null) {
				writer.Write(' ');
				Options.WriteTo(writer);
			}
		}
	}
}
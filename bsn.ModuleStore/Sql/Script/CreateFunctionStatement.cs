using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script;
using bsn.ModuleStore.Sql.Script.Tokens;

[assembly: RuleTrim("<FunctionInlineSelect> ::= '(' <FunctionInlineSelect> ')'", "<FunctionInlineSelect>", SemanticTokenType = typeof(SqlToken))]
[assembly: RuleTrim("<OrderClause> ::= ORDER BY <OrderList>", "<OrderList>", SemanticTokenType = typeof(SqlToken))]

namespace bsn.ModuleStore.Sql.Script {
	public abstract class CreateFunctionStatement<TBody>: CreateStatement where TBody: Statement {
		private readonly TBody body;
		private readonly Qualified<SchemaName, FunctionName> functionName;
		private readonly FunctionOption option;
		private readonly List<FunctionParameter> parameters;

		protected CreateFunctionStatement(Qualified<SchemaName, FunctionName> functionName, Sequence<FunctionParameter> parameters, FunctionOptionToken option, TBody body) {
			Debug.Assert(functionName != null);
			Debug.Assert(body != null);
			this.functionName = functionName;
			this.parameters = parameters.ToList();
			this.option = option.FunctionOption;
			this.body = body;
		}

		public override string ObjectName {
			get {
				return functionName.Name.Value;
			}
		}

		public TBody Body {
			get {
				return body;
			}
		}

		public Qualified<SchemaName, FunctionName> FunctionName {
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

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CREATE FUNCTION ");
			writer.WriteScript(functionName, WhitespacePadding.None);
			writer.Write(" (");
			writer.IncreaseIndent();
			writer.WriteScriptSequence(parameters, WhitespacePadding.NewlineBefore, ", ");
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.WriteLine(")");
			writer.Write("RETURNS ");
		}
	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script;
using bsn.ModuleStore.Sql.Script.Tokens;

[assembly: RuleTrim("<FunctionInlineSelect> ::= '(' <FunctionInlineSelect> ')'", "<FunctionInlineSelect>", SemanticTokenType = typeof(SqlToken))]
[assembly: RuleTrim("<OrderClause> ::= ORDER BY <OrderList>", "<OrderList>", SemanticTokenType = typeof(SqlToken))]

namespace bsn.ModuleStore.Sql.Script {
	public abstract class CreateFunctionStatement<TBody>: CreateStatement, ICreateOrAlterStatement where TBody: SqlScriptableToken {
		private readonly TBody body;
		private readonly Qualified<SchemaName, FunctionName> functionName;
		private readonly OptionToken option;
		private readonly List<FunctionParameter> parameters;

		protected CreateFunctionStatement(Qualified<SchemaName, FunctionName> functionName, Sequence<FunctionParameter> parameters, OptionToken option, TBody body) {
			Debug.Assert(functionName != null);
			Debug.Assert(body != null);
			this.functionName = functionName;
			this.parameters = parameters.ToList();
			this.option = option;
			this.body = body;
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

		public override sealed ObjectCategory ObjectCategory {
			get {
				return ObjectCategory.Function;
			}
		}

		public override string ObjectName {
			get {
				return functionName.Name.Value;
			}
		}

		public OptionToken Option {
			get {
				return option;
			}
		}

		public IEnumerable<FunctionParameter> Parameters {
			get {
				return parameters;
			}
		}

		public override Statement CreateAlterStatement() {
			return new AlterOfCreateStatement<CreateFunctionStatement<TBody>>(this);
		}

		public override DropStatement CreateDropStatement() {
			return new DropFunctionStatement(functionName);
		}

		public override sealed void WriteTo(SqlWriter writer) {
			WriteToInternal(writer, "CREATE");
		}

		protected override sealed string GetObjectSchema() {
			return functionName.IsQualified ? functionName.Qualification.Value : string.Empty;
		}

		protected virtual void WriteToInternal(SqlWriter writer, string command) {
			WriteCommentsTo(writer);
			writer.Write(command);
			writer.Write(" FUNCTION ");
			writer.WriteScript(functionName, WhitespacePadding.None);
			writer.Write(" (");
			writer.IncreaseIndent();
			writer.WriteScriptSequence(parameters, WhitespacePadding.NewlineBefore, ", ");
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.WriteLine(")");
			writer.Write("RETURNS ");
		}

		void ICreateOrAlterStatement.WriteToInternal(SqlWriter writer, string command) {
			if (string.IsNullOrEmpty(command)) {
				throw new ArgumentNullException("command");
			}
			WriteToInternal(writer, command);
		}
	}
}
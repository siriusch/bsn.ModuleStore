using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class ExecuteParameter: SqlScriptableToken {}

	public sealed class ExecuteParameter<T>: ExecuteParameter where T: SqlScriptableToken {
		private readonly bool output;
		private readonly ParameterName parameterName;
		private readonly T value;

		[Rule("<ExecuteParameter> ::= <VariableName> <OptionalOutput>", typeof(VariableName))]
		[Rule("<ExecuteParameter> ::= <SystemVariableName> <OptionalOutput>", typeof(VariableName))]
		[Rule("<ExecuteParameter> ::= <Literal> <OptionalOutput>", typeof(Literal))]
		public ExecuteParameter(T value, Optional<UnreservedKeyword> output): this(null, value, output) {}

		[Rule("<ExecuteParameter> ::= <ParameterName> ~'=' <VariableName> <OptionalOutput>", typeof(VariableName))]
		[Rule("<ExecuteParameter> ::= <ParameterName> ~'=' <SystemVariableName> <OptionalOutput>", typeof(VariableName))]
		[Rule("<ExecuteParameter> ::= <ParameterName> ~'=' <Literal> <OptionalOutput>", typeof(Literal))]
		public ExecuteParameter(ParameterName parameterName, T value, Optional<UnreservedKeyword> output): base() {
			Debug.Assert(value != null);
			this.parameterName = parameterName;
			this.value = value;
			this.output = output.HasValue();
		}

		public bool Output {
			get {
				return output;
			}
		}

		public ParameterName ParameterName {
			get {
				return parameterName;
			}
		}

		public T Value {
			get {
				return value;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(ParameterName, WhitespacePadding.None, null, "=");
			writer.WriteScript(value, WhitespacePadding.None);
			if (Output) {
				writer.Write(" OUTPUT");
			}
		}
	}
}
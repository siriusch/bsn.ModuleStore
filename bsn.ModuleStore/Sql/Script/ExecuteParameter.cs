using System;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class ExecuteParameter: SqlToken, IScriptable {
		protected ExecuteParameter() {}

		public abstract void WriteTo(TextWriter writer);
	}

	public sealed class ExecuteParameter<T>: ExecuteParameter where T: SqlToken, IScriptable {
		private bool output;
		private ParameterName parameterName;
		private readonly T value;

		[Rule("<ExecuteParameter> ::= <VariableName> <OptionalOutput>", typeof(VariableName))]
		[Rule("<ExecuteParameter> ::= <SystemVariableName> <OptionalOutput>", typeof(VariableName))]
		[Rule("<ExecuteParameter> ::= <Literal> <OptionalOutput>", typeof(Literal))]
		public ExecuteParameter(T value, Optional<OutputToken> output): this(null, value, output) {}

		[Rule("<ExecuteParameter> ::= <ParameterName> '=' <VariableName> <OptionalOutput>", typeof(VariableName), ConstructorParameterMapping = new[] {0, 2, 3})]
		[Rule("<ExecuteParameter> ::= <ParameterName> '=' <SystemVariableName> <OptionalOutput>", typeof(VariableName), ConstructorParameterMapping = new[] {0, 2, 3})]
		[Rule("<ExecuteParameter> ::= <ParameterName> '=' <Literal> <OptionalOutput>", typeof(Literal), ConstructorParameterMapping = new[] {0, 2, 3})]
		public ExecuteParameter(ParameterName parameterName, T value, Optional<OutputToken> output): base() {
			if (value == null) {
				throw new ArgumentNullException("value");
			}
			this.parameterName = parameterName;
			this.value = value;
			this.output = output.HasValue();
		}

		public T Value {
			get {
				return value;
			}
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

		public override void WriteTo(TextWriter writer) {
			writer.WriteScript(ParameterName, null, "=");
			writer.WriteScript(value);
			if (Output) {
				writer.Write(" OUTPUT");
			}
		}
	}
}
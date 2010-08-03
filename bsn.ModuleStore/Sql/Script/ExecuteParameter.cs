using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExecuteParameter: SqlToken {}

	public class ExecuteParameter<T>: ExecuteParameter where T: SqlToken {
		private readonly Output output;
		private readonly ParameterName parameterName;
		private readonly T value;

		[Rule("<ExecuteParameter> ::= <VariableName> <OptionalOutput>", typeof(VariableName))]
		[Rule("<ExecuteParameter> ::= <SystemVariableName> <OptionalOutput>", typeof(VariableName))]
		[Rule("<ExecuteParameter> ::= <Literal> <OptionalOutput>", typeof(Literal))]
		public ExecuteParameter(T value, Optional<Output> output): this(null, value, output) {}

		[Rule("<ExecuteParameter> ::= <ParameterName> '=' <VariableName> <OptionalOutput>", typeof(VariableName), ConstructorParameterMapping = new[] {0, 2, 3})]
		[Rule("<ExecuteParameter> ::= <ParameterName> '=' <SystemVariableName> <OptionalOutput>", typeof(VariableName), ConstructorParameterMapping = new[] {0, 2, 3})]
		[Rule("<ExecuteParameter> ::= <ParameterName> '=' <Literal> <OptionalOutput>", typeof(Literal), ConstructorParameterMapping = new[] {0, 2, 3})]
		public ExecuteParameter(ParameterName parameterName, T value, Optional<Output> output) {
			if (value == null) {
				throw new ArgumentNullException("value");
			}
			this.parameterName = parameterName;
			this.value = value;
			this.output = output;
		}
	}
}
using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class FunctionParameter: SqlToken {
		private readonly Literal defaultValue;
		private readonly ParameterName parameterName;
		private readonly Qualified<TypeName> parameterTypeName;

		[Rule("<FunctionParameter> ::= <ParameterName> <OptionalAs> <TypeNameQualified> <OptionalDefault>", ConstructorParameterMapping = new[] {0, 2, 3})]
		public FunctionParameter(ParameterName parameterName, Qualified<TypeName> parameterTypeName, Optional<Literal> defaultValue) {
			if (parameterName == null) {
				throw new ArgumentNullException("parameterName");
			}
			if (parameterTypeName == null) {
				throw new ArgumentNullException("parameterTypeName");
			}
			this.parameterName = parameterName;
			this.parameterTypeName = parameterTypeName;
			this.defaultValue = defaultValue;
		}

		public override void WriteTo(TextWriter writer) {
			parameterName.WriteTo(writer);
			writer.Write(" ");
			parameterTypeName.WriteTo(writer);
			if (defaultValue != null) {
				writer.Write(" = ");
				defaultValue.WriteTo(writer);
			}
		}
	}
}
using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ProcedureParameter: SqlToken {
		private readonly Literal defaultValue;
		private readonly Output output;
		private readonly ParameterName parameterName;
		private readonly Qualified<TypeName> parameterTypeName;
		private readonly bool readOnly;
		private readonly Varying varying;

		[Rule("<ProcedureParameter> ::= <ParameterName> <TypeNameQualified> <OptionalVarying> <OptionalDefault> <OptionalOutput> <OptionalReadonly>")]
		public ProcedureParameter(ParameterName parameterName, Qualified<TypeName> parameterTypeName, Optional<Varying> varying, Optional<Literal> defaultValue, Optional<Output> output, Optional<Identifier> readonlyIdentifier) {
			if (parameterName == null) {
				throw new ArgumentNullException("parameterName");
			}
			if (parameterTypeName == null) {
				throw new ArgumentNullException("parameterTypeName");
			}
			this.parameterName = parameterName;
			this.parameterTypeName = parameterTypeName;
			this.varying = varying;
			this.defaultValue = defaultValue;
			this.output = output;
			if (readonlyIdentifier != null) {
				if (string.Equals(readonlyIdentifier.Value.Value, "READONLY", StringComparison.OrdinalIgnoreCase)) {
					readOnly = true;
				} else {
					Debug.Fail("READONLY expected");
				}
			}
		}

		public override void WriteTo(TextWriter writer) {
			parameterName.WriteTo(writer);
			writer.Write(' ');
			parameterTypeName.WriteTo(writer);
			if (varying != null) {
				writer.Write(' ');
				varying.WriteTo(writer);
			}
			if (defaultValue != null) {
				writer.Write(" = ");
				defaultValue.WriteTo(writer);
			}
			if (output != null) {
				writer.Write(' ');
				output.WriteTo(writer);
			}
			if (readOnly) {
				writer.Write(" READONLY");
			}
		}
	}
}
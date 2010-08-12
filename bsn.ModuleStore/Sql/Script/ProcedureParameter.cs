using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ProcedureParameter: SqlScriptableToken {
		private readonly Literal defaultValue;
		private readonly bool output;
		private readonly ParameterName parameterName;
		private readonly Qualified<TypeName> parameterTypeName;
		private readonly bool readOnly;
		private readonly bool varying;

		[Rule("<ProcedureParameter> ::= <ParameterName> <TypeNameQualified> <OptionalVarying> <OptionalDefault> <OptionalOutput> <OptionalReadonly>")]
		public ProcedureParameter(ParameterName parameterName, Qualified<TypeName> parameterTypeName, Optional<VaryingToken> varying, Optional<Literal> defaultValue, Optional<OutputToken> output, Optional<Identifier> readonlyIdentifier) {
			Debug.Assert(parameterName != null);
			Debug.Assert(parameterTypeName != null);
			this.parameterName = parameterName;
			this.parameterTypeName = parameterTypeName;
			this.varying = varying.HasValue();
			this.defaultValue = defaultValue;
			this.output = output.HasValue();
			if (readonlyIdentifier.HasValue()) {
				if (string.Equals(readonlyIdentifier.Value.Value, "READONLY", StringComparison.OrdinalIgnoreCase)) {
					readOnly = true;
				} else {
					Debug.Fail("READONLY expected");
				}
			}
		}

		public Literal DefaultValue {
			get {
				return defaultValue;
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

		public Qualified<TypeName> ParameterTypeName {
			get {
				return parameterTypeName;
			}
		}

		public bool ReadOnly {
			get {
				return readOnly;
			}
		}

		public bool Varying {
			get {
				return varying;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(parameterName, WhitespacePadding.None);
			writer.WriteScript(parameterTypeName, WhitespacePadding.SpaceBefore);
			if (varying) {
				writer.Write(" VARYING");
			}
			writer.WriteScript(defaultValue, WhitespacePadding.None, "=", null);
			if (output) {
				writer.Write(" OUTPUT");
			}
			if (readOnly) {
				writer.Write(" READONLY");
			}
		}
	}
}
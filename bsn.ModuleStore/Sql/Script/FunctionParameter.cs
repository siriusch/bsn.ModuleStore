using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class FunctionParameter: SqlScriptableToken {
		private readonly Literal defaultValue;
		private readonly ParameterName parameterName;
		private readonly Qualified<SchemaName, TypeName> parameterTypeName;

		[Rule("<FunctionParameter> ::= <ParameterName> <OptionalAs> <TypeNameQualified> <OptionalDefault>", ConstructorParameterMapping = new[] {0, 2, 3})]
		public FunctionParameter(ParameterName parameterName, Qualified<SchemaName, TypeName> parameterTypeName, Optional<Literal> defaultValue) {
			Debug.Assert(parameterName != null);
			Debug.Assert(parameterTypeName != null);
			this.parameterName = parameterName;
			this.parameterTypeName = parameterTypeName;
			this.defaultValue = defaultValue;
		}

		public Literal DefaultValue {
			get {
				return defaultValue;
			}
		}

		public ParameterName ParameterName {
			get {
				return parameterName;
			}
		}

		public Qualified<SchemaName, TypeName> ParameterTypeName {
			get {
				return parameterTypeName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(parameterName, WhitespacePadding.SpaceAfter);
			writer.WriteScript(parameterTypeName, WhitespacePadding.None);
			writer.WriteScript(defaultValue, WhitespacePadding.None, "=", null);
		}
	}
}
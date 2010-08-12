using System;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionValue<T>: Expression where T: SqlScriptableToken {
		private readonly T valueSource;

		[Rule("<Value> ::= <SystemVariableName>", typeof(VariableName))]
		[Rule("<Value> ::= <VariableName>", typeof(VariableName))]
		[Rule("<Value> ::= <ColumnNameQualified>", typeof(Qualified<ColumnName>))]
		public ExpressionValue(T valueSource) {
			Debug.Assert(valueSource != null);
			this.valueSource = valueSource;
		}

		public T ValueSource {
			get {
				return valueSource;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(valueSource, WhitespacePadding.None);
		}
	}
}
using System;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionValue<T>: Expression where T: SqlToken, IScriptable {
		private readonly T valueSource;

		[Rule("<Value> ::= <SystemVariableName>", typeof(VariableName))]
		[Rule("<Value> ::= <VariableName>", typeof(VariableName))]
		[Rule("<Value> ::= <ColumnNameQualified>", typeof(Qualified<ColumnName>))]
		public ExpressionValue(T valueSource) {
			if (valueSource == null) {
				throw new ArgumentNullException("valueSource");
			}
			this.valueSource = valueSource;
		}

		public T ValueSource {
			get {
				return valueSource;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteScript(valueSource);
		}
	}
}
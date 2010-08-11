using System;
using System.Globalization;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TypeNameWithPrecision: TypeNameExtended {
		private readonly long precision;

		[Rule("<TypeName> ::= Id '(' <IntegerLiteral> ')'", ConstructorParameterMapping = new[] {0, 2})]
		public TypeNameWithPrecision(SqlIdentifier identifier, IntegerLiteral precision)
				: base(identifier) {
			if (precision == null) {
				throw new ArgumentNullException("precision");
			}
			this.precision = precision.Value;
		}

		public long Precision {
			get {
				return precision;
			}
		}

		protected override void WriteArguments(TextWriter writer) {
			writer.Write(precision.ToString(NumberFormatInfo.InvariantInfo));
		}
	}
}
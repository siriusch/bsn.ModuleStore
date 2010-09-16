using System;
using System.Diagnostics;
using System.Globalization;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TypeNameWithPrecision: TypeNameExtended {
		private readonly long precision;

		[Rule("<TypeName> ::= Id ~'(' <IntegerLiteral> ~')'")]
		public TypeNameWithPrecision(SqlIdentifier identifier, IntegerLiteral precision): base(identifier) {
			Debug.Assert(precision != null);
			this.precision = precision.Value;
		}

		public long Precision {
			get {
				return precision;
			}
		}

		protected override void WriteArguments(SqlWriter writer) {
			writer.Write(precision.ToString(NumberFormatInfo.InvariantInfo));
		}
	}
}
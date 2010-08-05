using System;
using System.Globalization;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("IntegerLiteral")]
	public class IntegerLiteral: NumericLiteral<long> {
		public IntegerLiteral(string value): this(long.Parse(value, NumberFormatInfo.InvariantInfo)) {}

		protected IntegerLiteral(long value): base(value) {}

		public override double AsDouble {
			get {
				return Value;
			}
		}
	}
}
using System;
using System.Globalization;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("IntegerLiteral")]
	public class IntegerLiteral: NumericLiteral<int> {
		public IntegerLiteral(string value): this(int.Parse(value, NumberFormatInfo.InvariantInfo)) {}

		protected IntegerLiteral(int value): base(value) {}

		public override double AsDouble {
			get {
				return Value;
			}
		}
	}
}
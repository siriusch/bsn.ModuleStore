using System;
using System.Globalization;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("RealLiteral")]
	public class RealLiteral: NumericLiteral<double> {
		public RealLiteral(string value): base(double.Parse(value, NumberFormatInfo.InvariantInfo)) {}

		public override double AsDouble {
			get {
				return Value;
			}
		}
	}
}
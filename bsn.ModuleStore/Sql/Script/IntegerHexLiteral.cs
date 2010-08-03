using System;
using System.Globalization;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("HexLiteral")]
	public class IntegerHexLiteral: IntegerLiteral {
		public IntegerHexLiteral(string value): base(int.Parse(value.Substring(2), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo)) {}

		public override void WriteTo(TextWriter writer) {
			writer.Write(string.Format(CultureInfo.InvariantCulture, "0x{0:X}", Value));
		}
	}
}
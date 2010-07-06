using System;
using System.IO;
using System.Text.RegularExpressions;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlQuotedName: SqlName {
		private static readonly char[] brackets = new[] {']', '['};
		private static readonly Regex rxUnquote = new Regex(@"^(\[(?<name>[^\[\]]+)\]|""(?<name>[^""]+""|(?<name>\S+)$", RegexOptions.CultureInvariant|RegexOptions.ExplicitCapture);

		protected SqlQuotedName(string name): base(rxUnquote.Match(name).Groups["name"].Value) {}

		public override void WriteTo(TextWriter writer) {
			if (Value.LastIndexOfAny(brackets) < 0) {
				writer.Write('[');
				writer.Write(Value);
				writer.Write(']');
			} else {
				writer.Write('"');
				writer.Write(Value);
				writer.Write('"');
			}
		}
	}
}
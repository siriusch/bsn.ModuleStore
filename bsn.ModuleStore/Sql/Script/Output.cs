using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("OUTPUT")]
	public class Output: SqlToken {
		public override void WriteTo(TextWriter writer) {
			writer.Write("OUTPUT");
		}
	}
}
using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("PRIMARY")]
	public class Primary: SqlToken {
		public override void WriteTo(TextWriter writer) {
			writer.Write("PRIMARY");
		}
	}
}
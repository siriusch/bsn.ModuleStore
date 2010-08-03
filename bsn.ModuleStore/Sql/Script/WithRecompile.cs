using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("WITH_RECOMPILE")]
	public class WithRecompile: SqlToken {
		public override void WriteTo(TextWriter writer) {
			writer.Write("WITH RECOMPILE");
		}
	}
}
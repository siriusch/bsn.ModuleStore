using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("WITH_CHECK_OPTION")]
	public class WithCheckOption: SqlToken {
		public override void WriteTo(TextWriter writer) {
			writer.Write("WITH CHECK OPTION");
		}
	}
}
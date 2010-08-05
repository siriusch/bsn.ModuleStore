using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("WITH_VIEW_METADATA")]
	public class WithViewMetadata: SqlToken {
		public override void WriteTo(TextWriter writer) {
			writer.Write("WITH VIEW_METADATA");
		}
	}
}
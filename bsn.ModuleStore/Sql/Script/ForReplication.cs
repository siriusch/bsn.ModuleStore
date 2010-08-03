using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("FOR_REPLICATION")]
	public class ForReplication: SqlToken {
		public override void WriteTo(TextWriter writer) {
			writer.Write("FOR REPLICATION");
		}
	}
}
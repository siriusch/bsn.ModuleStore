using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("VARYING")]
	public class Varying: SqlToken {
		public override void WriteTo(TextWriter writer) {
			writer.Write("VARYING");
		}
	}
}
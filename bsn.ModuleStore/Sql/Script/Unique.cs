using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("UNIQUE")]
	public class Unique: SqlToken {
		public override void WriteTo(TextWriter writer) {
			writer.Write("UNIQUE");
		}
	}
}
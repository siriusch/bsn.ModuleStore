using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("Id")]
	public sealed class Identifier: SqlIdentifier, IScriptable {
		public Identifier(string id): base(id) {}

		public void WriteTo(SqlWriter writer) {
			writer.Write(Original);
		}
	}
}
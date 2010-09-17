using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("NULL")]
	public sealed class NullLiteral: Literal {
		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("NULL");
		}
	}
}
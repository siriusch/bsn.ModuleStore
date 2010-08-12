using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("OR")]
	[Terminal("AND")]
	public sealed class OperationNameToken: OperationToken {
		public OperationNameToken(string operation): base(operation) {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write(' ');
			base.WriteTo(writer);
			writer.Write(' ');
		}
	}
}
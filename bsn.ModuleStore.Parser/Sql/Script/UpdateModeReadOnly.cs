using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("READ_ONLY")]
	public sealed class UpdateModeReadOnly: UpdateMode {
		public UpdateModeReadOnly() {}

		public override UpdateModeKind Kind {
			get {
				return UpdateModeKind.ReadOnly;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("READ ONLY");
		}
	}
}
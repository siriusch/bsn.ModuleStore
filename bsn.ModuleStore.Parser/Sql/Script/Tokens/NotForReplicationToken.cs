using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class NotForReplicationToken: ReplicationToken {
		[Rule("<OptionalNotForReplication> ::= ~NOT ~FOR ~REPLICATION")]
		public NotForReplicationToken() {}

		public override bool? ForReplication {
			get {
				return false;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("NOT FOR REPLICATION");
		}
	}
}
using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class ForReplicationToken: ReplicationToken {
		[Rule("<ProcedureFor> ::= ~FOR ~REPLICATION")]
		public ForReplicationToken() {}

		public override bool? ForReplication {
			get {
				return true;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("FOR REPLICATION");
		}
	}
}
using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class ReplicationToken: SqlScriptableToken, IOptional {
		[Rule("<OptionalNotForReplication> ::=")]
		[Rule("<ProcedureFor> ::=")]
		public ReplicationToken() {}

		public virtual bool? ForReplication {
			get {
				return null;
			}
		}

		public override void WriteTo(SqlWriter writer) {}

		public bool HasValue {
			get {
				return ForReplication.HasValue;
			}
		}
	}
}
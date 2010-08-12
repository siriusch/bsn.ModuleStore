using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DestinationRowset: SqlScriptableToken {
		private readonly SqlName name;

		[Rule("<DestinationRowset> ::= <VariableName>")]
		[Rule("<DestinationRowset> ::= <TableName>")]
		public DestinationRowset(SqlName name) {
			Debug.Assert(name != null);
			this.name = name;
		}

		public SqlName Name {
			get {
				return name;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(name, WhitespacePadding.None);
		}
	}
}
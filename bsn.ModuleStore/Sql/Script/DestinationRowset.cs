using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DestinationRowset: SqlToken, IScriptable {
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

		public void WriteTo(TextWriter writer) {
			writer.WriteScript(name);
		}
	}
}
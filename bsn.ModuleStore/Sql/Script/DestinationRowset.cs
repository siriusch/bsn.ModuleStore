using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DestinationRowset: SqlToken, IScriptable {
		private readonly SqlName name;

		[Rule("<DestinationRowset> ::= <VariableName>")]
		[Rule("<DestinationRowset> ::= <TableName>")]
		public DestinationRowset(SqlName name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
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
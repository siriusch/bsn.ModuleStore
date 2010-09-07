using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class DestinationRowset: SqlScriptableToken {
	}

	public sealed class DestinationRowset<T>: DestinationRowset where T: SqlScriptableToken {
		private readonly T name;

		[Rule("<DestinationRowset> ::= <VariableName>", typeof(VariableName))]
		[Rule("<DestinationRowset> ::= <TableNameQualified>", typeof(Qualified<SchemaName, TableName>))]
		public DestinationRowset(T name) {
			Debug.Assert(name != null);
			this.name = name;
		}

		public T Name {
			get {
				return name;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(name, WhitespacePadding.None);
		}
	}
}
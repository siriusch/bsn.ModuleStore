using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class DestinationRowset: SqlScriptableToken {
	}

	public sealed class DestinationRowset<T>: DestinationRowset where T: SqlScriptableToken {
		private readonly T name;
		private readonly TableHintGroup tableHints;

		[Rule("<DestinationRowset> ::= <VariableName>", typeof(VariableName))]
		public DestinationRowset(T name): this(name, null) {}

		[Rule("<DestinationRowset> ::= <TableNameQualified> <TableHintGroup>", typeof(Qualified<SchemaName, TableName>))]
		public DestinationRowset(T name, TableHintGroup tableHints) {
			Debug.Assert(name != null);
			this.name = name;
			this.tableHints = tableHints;
		}

		public T Name {
			get {
				return name;
			}
		}

		public TableHintGroup TableHints {
			get {
				return tableHints;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(name, WhitespacePadding.None);
			writer.WriteScript(tableHints, WhitespacePadding.SpaceBefore);
		}
	}
}
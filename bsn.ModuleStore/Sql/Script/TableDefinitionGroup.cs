using System;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TableDefinitionGroup: SqlToken {
		private readonly Sequence<TableDefinition> definitions;

		[Rule("<TableDefinitionGroup> ::= '(' <TableDefinitionList> ')'", ConstructorParameterMapping = new[] {1})]
		public TableDefinitionGroup(Sequence<TableDefinition> definitions) {
			if (definitions == null) {
				throw new ArgumentNullException("definitions");
			}
			this.definitions = definitions;
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write('(');
			string separator = string.Empty;
			foreach (TableDefinition definitionItem in definitions) {
				writer.Write(separator);
				definitionItem.WriteTo(writer);
				separator = ", ";
			}
			writer.Write(')');
		}
	}
}
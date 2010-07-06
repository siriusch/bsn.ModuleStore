using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TableDefinition: SqlToken {
		private readonly Sequence<TableDefinitionItem> definitions;

		[Rule("<TableDefinitionGroup> ::= '(' <TableTypeDefinitionList> ')'", ConstructorParameterMapping = new[] {1})]
		public TableDefinition(Sequence<TableDefinitionItem> definitions) {
			if (definitions == null) {
				throw new ArgumentNullException("definitions");
			}
			this.definitions = definitions;
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write('(');
			string separator = string.Empty;
			foreach (TableDefinitionItem definitionItem in definitions) {
				writer.Write(separator);
				definitionItem.WriteTo(writer);
				separator = ", ";
			}
			writer.Write(')');
		}
	}
}
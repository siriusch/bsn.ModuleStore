using System;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DeclareTableStatement: VariableDeclaration {
		private readonly TableDefinitionGroup tableDefinition;

		[Rule("<DeclareStatement> ::= DECLARE <VariableName> <OptionalAs> TABLE <TableDefinitionGroup>", ConstructorParameterMapping = new[] {1, 4})]
		public DeclareTableStatement(VariableName name, TableDefinitionGroup tableDefinition): base(name) {
			if (tableDefinition == null) {
				throw new ArgumentNullException("tableDefinition");
			}
			this.tableDefinition = tableDefinition;
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("DECLARE ");
			Variable.WriteTo(writer);
			writer.Write(" TABLE ");
			tableDefinition.WriteTo(writer);
		}
	}
}
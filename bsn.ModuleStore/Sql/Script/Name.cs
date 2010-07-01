using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class Name: SqlToken {
		private readonly string name;

		[Rule("<AliasName> ::= Id")]
		[Rule("<CollationName> ::= Id")]
		[Rule("<ColumnName> ::= Id")]
		[Rule("<ConstraintName> ::= Id")]
		[Rule("<CursorName> ::= Id")]
		[Rule("<FunctionName> ::= Id")]
		[Rule("<FunctionName> ::= SystemFuncId")]
		[Rule("<IndexName> ::= Id")]
		[Rule("<LabelName> ::= Id")]
		[Rule("<ParameterName> ::= LocalId")]
		[Rule("<ProcedureName> ::= Id")]
		[Rule("<TableName> ::= Id")]
		[Rule("<TableName> ::= TempTableId")]
		[Rule("<TriggerName> ::= Id")]
		[Rule("<SchemaName> ::= Id")]
		[Rule("<SystemVariableName> ::= SystemVarId")]
		[Rule("<VariableName> ::= LocalId")]
		[Rule("<ViewName> ::= Id")]
		[Rule("<XmlElementName> ::= Id")]
		[Rule("<XmlSchemaCollectionName> ::= Id")]
		public Name(Identifier identifier) {
			name = identifier.Name;
		}

		[Rule("<ColumnWild> ::= '*'")]
		public Name(OperationToken star) {
			name = "*";
		}

		public override string ToString() {
			return name;
		}
	}
}

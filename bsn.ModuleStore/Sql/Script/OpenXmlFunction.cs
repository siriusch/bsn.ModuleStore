using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class OpenxmlFunction: SqlToken {
		private readonly VariableName variableName;
		private readonly SqlToken stringValue;
		private readonly IntegerLiteral flags;
		private readonly OpenxmlSchema schema;

		[Rule("<Openxml> ::= OPENXML '(' <VariableName> ',' <StringValue> ')' <OptionalOpenxmlSchema>", ConstructorParameterMapping=new[] { 2, 4, 6 })]
		public OpenxmlFunction(VariableName variableName, SqlToken stringValue, Optional<OpenxmlSchema> schema): this(variableName, stringValue, null, schema) {
		}

		[Rule("<Openxml> ::= OPENXML '(' <VariableName> ',' <StringValue> ',' <IntegerLiteral> ')' <OptionalOpenxmlSchema>", ConstructorParameterMapping=new[] { 2, 4, 6, 8 })]
		public OpenxmlFunction(VariableName variableName, SqlToken stringValue, IntegerLiteral flags, Optional<OpenxmlSchema> schema) {
			if (variableName == null) {
				throw new ArgumentNullException("variableName");
			}
			if (stringValue == null) {
				throw new ArgumentNullException("stringValue");
			}
			if (schema == null) {
				throw new ArgumentNullException("schema");
			}
			this.variableName = variableName;
			this.stringValue = stringValue;
			this.flags = flags;
			this.schema = schema;
		}
	}
}

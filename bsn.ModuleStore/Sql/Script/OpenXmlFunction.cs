using System;
using System.Globalization;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class OpenxmlFunction: SqlToken, IScriptable {
		private readonly int flags;
		private readonly OpenxmlSchema schema;
		private readonly IScriptable stringValue;
		private readonly VariableName variableName;

		[Rule("<Openxml> ::= OPENXML '(' <VariableName> ',' <StringValue> ')' <OptionalOpenxmlSchema>", ConstructorParameterMapping = new[] {2, 4, 6})]
		public OpenxmlFunction(VariableName variableName, IScriptable stringValue, Optional<OpenxmlSchema> schema): this(variableName, stringValue, null, schema) {}

		[Rule("<Openxml> ::= OPENXML '(' <VariableName> ',' <StringValue> ',' <IntegerLiteral> ')' <OptionalOpenxmlSchema>", ConstructorParameterMapping = new[] {2, 4, 6, 8})]
		public OpenxmlFunction(VariableName variableName, IScriptable stringValue, IntegerLiteral flags, Optional<OpenxmlSchema> schema) {
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
			this.flags = (int)flags.Value;
			this.schema = schema;
		}

		public int Flags {
			get {
				return flags;
			}
		}

		public OpenxmlSchema Schema {
			get {
				return schema;
			}
		}

		public IScriptable StringValue {
			get {
				return stringValue;
			}
		}

		public VariableName VariableName {
			get {
				return variableName;
			}
		}

		public void WriteTo(TextWriter writer) {
			writer.Write("OPENXML (");
			writer.WriteScript(variableName);
			writer.Write(", ");
			stringValue.WriteTo(writer);
			if (flags != 0) {
				writer.Write(", ");
				writer.Write(flags.ToString(NumberFormatInfo.InvariantInfo));
			}
			writer.Write(')');
			writer.WriteScript(schema, " ", null);
		}
	}
}
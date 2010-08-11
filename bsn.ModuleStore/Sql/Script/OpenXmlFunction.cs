using System;
using System.Diagnostics;
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

		[Rule("<Openxml> ::= OPENXML '(' <VariableName> ',' <StringLiteral> ')' <OptionalOpenxmlSchema>", ConstructorParameterMapping = new[] {2, 4, 6})]
		[Rule("<Openxml> ::= OPENXML '(' <VariableName> ',' <VariableName> ')' <OptionalOpenxmlSchema>", ConstructorParameterMapping = new[] {2, 4, 6})]
		public OpenxmlFunction(VariableName variableName, IScriptable stringValue, Optional<OpenxmlSchema> schema): this(variableName, stringValue, null, schema) {}

		[Rule("<Openxml> ::= OPENXML '(' <VariableName> ',' <StringLiteral> ',' <IntegerLiteral> ')' <OptionalOpenxmlSchema>", ConstructorParameterMapping = new[] {2, 4, 6, 8})]
		[Rule("<Openxml> ::= OPENXML '(' <VariableName> ',' <VariableName> ',' <IntegerLiteral> ')' <OptionalOpenxmlSchema>", ConstructorParameterMapping = new[] {2, 4, 6, 8})]
		public OpenxmlFunction(VariableName variableName, IScriptable stringValue, IntegerLiteral flags, Optional<OpenxmlSchema> schema) {
			Debug.Assert(variableName != null);
			Debug.Assert(stringValue != null);
			Debug.Assert(schema != null);
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

		public void WriteTo(SqlWriter writer) {
			writer.Write("OPENXML (");
			writer.WriteScript(variableName, WhitespacePadding.None);
			writer.Write(", ");
			stringValue.WriteTo(writer);
			if (flags != 0) {
				writer.Write(", ");
				writer.Write(flags.ToString(NumberFormatInfo.InvariantInfo));
			}
			writer.Write(')');
			writer.WriteScript(schema, WhitespacePadding.SpaceBefore);
		}
	}
}
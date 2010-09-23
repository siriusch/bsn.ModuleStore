using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class XmlDirective: SqlScriptableToken {
		private readonly StringLiteral elementName;
		private readonly Identifier key;
		private readonly Identifier value;

		[Rule("<XmlDirective> ::= Id Id")]
		public XmlDirective(Identifier key, Identifier value): this(key) {
			this.value = value;
		}

		[Rule("<XmlDirective> ::= Id <OptionalElementName>")]
		public XmlDirective(Identifier key, Optional<StringLiteral> elementName): this(key) {
			this.elementName = elementName;
		}

		private XmlDirective(Identifier key) {
			Debug.Assert(key != null);
			this.key = key;
		}

		public StringLiteral ElementName {
			get {
				return elementName;
			}
		}

		public Identifier Key {
			get {
				return key;
			}
		}

		public Identifier Value {
			get {
				return value;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(key, WhitespacePadding.None);
			writer.WriteScript(value, WhitespacePadding.SpaceBefore);
			if (elementName != null) {
				writer.Write('(');
				writer.WriteScript(elementName, WhitespacePadding.None);
				writer.Write(')');
			}
		}
	}
}
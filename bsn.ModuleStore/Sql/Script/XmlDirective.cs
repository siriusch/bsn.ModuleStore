using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class XmlDirective: SqlToken {
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
			if (key == null) {
				throw new ArgumentNullException("key");
			}
			this.key = key;
		}
	}
}
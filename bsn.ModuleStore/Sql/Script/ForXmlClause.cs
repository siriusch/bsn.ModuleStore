using System;
using System.Diagnostics;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ForXmlClause: ForClause {
		private readonly Sequence<XmlDirective> directives;
		private readonly StringLiteral elementName;
		private readonly ForXmlKind kind;

		[Rule("<ForClause> ::= FOR_XML_AUTO")]
		[Rule("<ForClause> ::= FOR_XML_EXPLICIT")]
		public ForXmlClause(IToken xmlToken): this(xmlToken, null, null) {}

		[Rule("<ForClause> ::= FOR_XML_RAW <OptionalElementName>")]
		[Rule("<ForClause> ::= FOR_XML_PATH <OptionalElementName>")]
		public ForXmlClause(IToken xmlToken, Optional<StringLiteral> elementName): this(xmlToken, elementName, null) {}

		[Rule("<ForClause> ::= FOR_XML_AUTO <XmlDirectiveList>")]
		[Rule("<ForClause> ::= FOR_XML_EXPLICIT <XmlDirectiveList>")]
		public ForXmlClause(IToken xmlToken, Sequence<XmlDirective> directives): this(xmlToken, null, directives) {}

		[Rule("<ForClause> ::= FOR_XML_RAW <OptionalElementName> <XmlDirectiveList>")]
		[Rule("<ForClause> ::= FOR_XML_PATH <OptionalElementName> <XmlDirectiveList>")]
		public ForXmlClause(IToken xmlToken, Optional<StringLiteral> elementName, Sequence<XmlDirective> directives) {
			this.directives = directives;
			switch (xmlToken.Symbol.Name) {
			case "FOR_XML_AUTO":
				kind = ForXmlKind.Auto;
				break;
			case "FOR_XML_RAW":
				kind = ForXmlKind.Raw;
				break;
			case "FOR_XML_EXPLICIT":
				kind = ForXmlKind.Explicit;
				break;
			case "FOR_XML_PATH":
				kind = ForXmlKind.Path;
				break;
			default:
				Debug.Fail("Unknown FOR XML kind");
				break;
			}
			this.elementName = elementName;
		}

		public Sequence<XmlDirective> Directives {
			get {
				return directives;
			}
		}

		public StringLiteral ElementName {
			get {
				return elementName;
			}
		}

		public ForXmlKind Kind {
			get {
				return kind;
			}
		}
	}
}
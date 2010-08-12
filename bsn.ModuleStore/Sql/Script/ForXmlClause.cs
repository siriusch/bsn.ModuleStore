using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ForXmlClause: ForClause {
		private readonly List<XmlDirective> directives;
		private readonly StringLiteral elementName;
		private readonly ForXmlKind kind;

		[Rule("<ForClause> ::= FOR_XML_AUTO")]
		[Rule("<ForClause> ::= FOR_XML_EXPLICIT")]
		public ForXmlClause(ForXmlToken xmlToken): this(xmlToken, null, null) {}

		[Rule("<ForClause> ::= FOR_XML_RAW <OptionalElementName>")]
		[Rule("<ForClause> ::= FOR_XML_PATH <OptionalElementName>")]
		public ForXmlClause(ForXmlToken xmlToken, Optional<StringLiteral> elementName): this(xmlToken, elementName, null) {}

		[Rule("<ForClause> ::= FOR_XML_AUTO <XmlDirectiveList>")]
		[Rule("<ForClause> ::= FOR_XML_EXPLICIT <XmlDirectiveList>")]
		public ForXmlClause(ForXmlToken xmlToken, Sequence<XmlDirective> directives): this(xmlToken, null, directives) {}

		[Rule("<ForClause> ::= FOR_XML_RAW <OptionalElementName> <XmlDirectiveList>")]
		[Rule("<ForClause> ::= FOR_XML_PATH <OptionalElementName> <XmlDirectiveList>")]
		public ForXmlClause(ForXmlToken xmlToken, Optional<StringLiteral> elementName, Sequence<XmlDirective> directives) {
			Debug.Assert(xmlToken != null);
			this.directives = directives.ToList();
			kind = xmlToken.Kind;
			this.elementName = elementName;
		}

		public List<XmlDirective> Directives {
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

		public override SelectFor SelectFor {
			get {
				return SelectFor.Xml;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteEnum(kind, WhitespacePadding.None);
			writer.WriteScript(elementName, WhitespacePadding.SpaceBefore);
			if (directives.Count > 0) {
				writer.Write(' ');
				writer.WriteSequence(directives, WhitespacePadding.None, ", ");
			}
		}
	}
}
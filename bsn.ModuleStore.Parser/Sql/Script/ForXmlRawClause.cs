using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ForXmlRawClause: ForXmlClause {
		[Rule("<ForClause> ::= ~FOR ~XML ~RAW <OptionalElementName> <XmlDirectiveList>")]
		public ForXmlRawClause(Optional<StringLiteral> elementName, Sequence<XmlDirective> directives): base(elementName, directives) {}

		public override ForXmlKind Kind {
			get {
				return ForXmlKind.Raw;
			}
		}

		protected override string KindSpecifier {
			get {
				return "RAW";
			}
		}
	}
}
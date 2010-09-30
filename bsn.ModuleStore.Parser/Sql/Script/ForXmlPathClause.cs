using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ForXmlPathClause: ForXmlClause {
		[Rule("<ForClause> ::= ~FOR ~XML ~PATH <OptionalElementName> <XmlDirectiveList>")]
		public ForXmlPathClause(Optional<StringLiteral> elementName, Sequence<XmlDirective> directives): base(elementName, directives) {}

		public override ForXmlKind Kind {
			get {
				return ForXmlKind.Path;
			}
		}

		protected override string KindSpecifier {
			get {
				return "PATH";
			}
		}
	}
}
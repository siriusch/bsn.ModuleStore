using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ForXmlAutoClause: ForXmlClause {
		[Rule("<ForClause> ::= ~FOR ~XML ~AUTO <XmlDirectiveList>")]
		public ForXmlAutoClause(Sequence<XmlDirective> directives): base(null, directives) {}

		public override ForXmlKind Kind {
			get {
				return ForXmlKind.Auto;
			}
		}

		protected override string KindSpecifier {
			get {
				return "AUTO";
			}
		}
	}
}
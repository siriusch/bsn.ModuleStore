using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ForXmlExplicitClause: ForXmlClause {
		[Rule("<ForClause> ::= ~FOR ~XML ~EXPLICIT <XmlDirectiveList>")]
		public ForXmlExplicitClause(Sequence<XmlDirective> directives): base(null, directives) {}

		public override ForXmlKind Kind {
			get {
				return ForXmlKind.Auto;
			}
		}

		protected override string KindSpecifier {
			get {
				return "EXPLICIT";
			}
		}
	}
}
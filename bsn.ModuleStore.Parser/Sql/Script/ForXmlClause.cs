using System;
using System.Collections.Generic;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class ForXmlClause: ForClause {
		private readonly List<XmlDirective> directives;
		private readonly StringLiteral elementName;

		protected ForXmlClause(StringLiteral elementName, Sequence<XmlDirective> directives) {
			this.directives = directives.ToList();
			this.elementName = elementName;
		}

		public IEnumerable<XmlDirective> Directives {
			get {
				return directives;
			}
		}

		public StringLiteral ElementName {
			get {
				return elementName;
			}
		}

		public abstract ForXmlKind Kind {
			get;
		}

		public override SelectFor SelectFor {
			get {
				return SelectFor.Xml;
			}
		}

		protected abstract String KindSpecifier {
			get;
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("FOR XML ");
			writer.Write(KindSpecifier);
			if (elementName != null) {
				writer.Write(" (");
				writer.WriteScript(elementName, WhitespacePadding.None);
				writer.Write(')');
			}
			if (directives.Count > 0) {
				writer.Write(", ");
				writer.WriteScriptSequence(directives, WhitespacePadding.None, ", ");
			}
		}
	}
}
using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class XmlNamespace: CommentContainerToken {
		private readonly StringLiteral namespaceUri;
		private readonly XmlNamespaceName xmlNamespaceName;

		[Rule("<XmlNamespace> ::= <StringLiteral> ~AS <XmlNamespaceName>")]
		public XmlNamespace(StringLiteral namespaceUri, XmlNamespaceName xmlNamespaceName) {
			Debug.Assert(namespaceUri != null);
			Debug.Assert(xmlNamespaceName != null);
			this.xmlNamespaceName = xmlNamespaceName;
			this.namespaceUri = namespaceUri;
		}

		public StringLiteral NamespaceUri {
			get {
				return namespaceUri;
			}
		}

		public XmlNamespaceName XmlNamespaceName {
			get {
				return xmlNamespaceName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteScript(namespaceUri, WhitespacePadding.None);
			writer.Write(" AS ");
			writer.WriteScript(xmlNamespaceName, WhitespacePadding.None);
		}
	}
}
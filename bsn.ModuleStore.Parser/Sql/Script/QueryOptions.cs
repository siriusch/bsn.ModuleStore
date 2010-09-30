using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class QueryOptions: CommentContainerToken, IOptional {
		private readonly List<CommonTableExpression> commonTableExpressions;
		private readonly List<XmlNamespace> namespaces;

		[Rule("<QueryOptions> ::= ~WITH ~XMLNAMESPACES ~'(' <XmlNamespaceList> ~')'")]
		public QueryOptions(Sequence<XmlNamespace> namespaces): this(namespaces, null) {}

		[Rule("<QueryOptions> ::= ~WITH <CTEList>")]
		public QueryOptions(Sequence<CommonTableExpression> commonTableExpressions): this(null, commonTableExpressions) {}

		[Rule("<QueryOptions> ::= ~WITH ~XMLNAMESPACES ~'(' <XmlNamespaceList> ~')' ~',' <CTEList>")]
		public QueryOptions(Sequence<XmlNamespace> namespaces, Sequence<CommonTableExpression> commonTableExpressions) {
			this.namespaces = namespaces.ToList();
			this.commonTableExpressions = commonTableExpressions.ToList();
		}

		[Rule("<QueryOptions> ::=")]
		public QueryOptions(): this(null, null) {}

		public IEnumerable<CommonTableExpression> CommonTableExpressions {
			get {
				return commonTableExpressions;
			}
		}

		public IEnumerable<XmlNamespace> Namespaces {
			get {
				return namespaces;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			Debug.Assert(HasValue);
			writer.Write("WITH ");
			if (namespaces.Count > 0) {
				writer.IncreaseIndent();
				writer.Write("XMLNAMESPACES (");
				writer.WriteScriptSequence(namespaces, WhitespacePadding.NewlineBefore, ",");
				writer.DecreaseIndent();
				writer.WriteLine();
				writer.Write(')');
				if (commonTableExpressions.Count > 0) {
					writer.WriteLine(",");
				}
			}
			writer.WriteScriptSequence(commonTableExpressions, WhitespacePadding.None, ","+Environment.NewLine);
		}

		public bool HasValue {
			get {
				return (namespaces.Count+commonTableExpressions.Count) > 0;
			}
		}
	}
}
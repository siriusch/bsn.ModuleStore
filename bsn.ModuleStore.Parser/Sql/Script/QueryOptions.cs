// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

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
			if (HasValue) {
				writer.WriteKeyword("WITH ");
				if (namespaces.Count > 0) {
					using (writer.Indent()) {
						writer.WriteKeyword("XMLNAMESPACES ");
						writer.Write('(');
						writer.WriteScriptSequence(namespaces, WhitespacePadding.NewlineBefore, w => w.Write(','));
					}
					writer.WriteLine();
					writer.Write(')');
					if (commonTableExpressions.Count > 0) {
						writer.WriteLine(",");
					}
				}
				writer.WriteScriptSequence(commonTableExpressions, WhitespacePadding.None, w => w.WriteLine(","));
			}
		}

		public bool HasValue {
			get {
				return (namespaces.Count+commonTableExpressions.Count) > 0;
			}
		}
	}
}

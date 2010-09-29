﻿using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class XmlNamespaceName: SqlQuotedName {
		[Rule("<XmlNamespaceName> ::= Id")]
		public XmlNamespaceName(Identifier identifier): base(identifier.Value) {}
	}
}
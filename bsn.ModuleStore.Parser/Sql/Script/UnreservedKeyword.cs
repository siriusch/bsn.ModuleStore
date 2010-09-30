using System;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("ACTION")]
	[Terminal("AFTER")]
	[Terminal("APPLY")]
	[Terminal("AUTO")]
	[Terminal("CALLED")]
	[Terminal("CALLER")]
	[Terminal("CAST")]
	[Terminal("CATCH")]
	[Terminal("CHANGE_TRACKING")]
	[Terminal("COLLECTION")]
	[Terminal("COMMITTED")]
	[Terminal("COUNT")]
	[Terminal("DISABLE")]
	[Terminal("ENABLE")]
	[Terminal("EXPLICIT")]
	[Terminal("EXTERNAL")]
	[Terminal("FULLTEXT")]
	[Terminal("INCLUDE")]
	[Terminal("INPUT")]
	[Terminal("INSTEAD")]
	[Terminal("LANGUAGE")]
	[Terminal("MANUAL")]
	[Terminal("MARK")]
	[Terminal("MAXRECURSION")]
	[Terminal("NAME")]
	[Terminal("NO")]
	[Terminal("ONLY")]
	[Terminal("OUTPUT")]
	[Terminal("PARTITION")]
	[Terminal("PATH")]
	[Terminal("PERSISTED")]
	[Terminal("POPULATION")]
	[Terminal("PROPERTY")]
	[Terminal("RAW")]
	[Terminal("RECOMPILE")]
	[Terminal("REPEATABLE")]
	[Terminal("RETURNS")]
	[Terminal("SCHEMABINDING")]
	[Terminal("SERVER")]
	[Terminal("TIES")]
	[Terminal("TRY")]
	[Terminal("TYPE")]
	[Terminal("UNCOMMITTED")]
	[Terminal("USING")]
	[Terminal("VALUE")]
	[Terminal("VIEW_METADATA")]
	[Terminal("WORK")]
	[Terminal("XML")]
	[Terminal("XMLNAMESPACES")]
	public class UnreservedKeyword: SqlToken {
		private readonly string originalValue;

		public UnreservedKeyword(string text): base() {
			originalValue = text;
		}

		public string OriginalValue {
			get {
				return originalValue;
			}
		}

		public Identifier AsIdentifier(Symbol identifierSymbol) {
			return new Identifier(originalValue, identifierSymbol, ((IToken)this).Position);
		}
	}
}
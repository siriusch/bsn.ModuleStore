using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TypeName: SqlQuotedName {
		[Rule("<TypeName> ::= Id")]
		public TypeName(SqlIdentifier identifier): base(identifier.Value) {}
	}
}
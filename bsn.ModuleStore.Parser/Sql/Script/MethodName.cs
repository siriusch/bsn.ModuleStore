using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class MethodName: SqlQuotedName {
		[Rule("<MethodName> ::= Id")]
		public MethodName(Identifier identifier): this(identifier.Value) {}

		internal MethodName(string name): base(name) {}
	}
}
using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SqlAssemblyName: SqlQuotedName {
		[Rule("<AssemblyName> ::= Id")]
		public SqlAssemblyName(Identifier identifier): this(identifier.Value) {}

		internal SqlAssemblyName(string name): base(name) {}
	}
}
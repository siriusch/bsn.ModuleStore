using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class SourceNestedSelectRowset: SourceRowset {
		[Rule("<SourceRowset> ::= '(' <SelectQuery> ')' <OptionalAlias>", ConstructorParameterMapping = new[] {1, 3})]
		public SourceNestedSelectRowset(SelectQuery select, Optional<AliasName> aliasName): base(aliasName) {}
	}
}
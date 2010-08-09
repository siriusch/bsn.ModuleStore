using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceNestedSelectRowset: SourceRowset {
		private readonly SelectQuery @select;

		[Rule("<SourceRowset> ::= '(' <SelectQuery> ')' <OptionalAlias>", ConstructorParameterMapping = new[] {1, 3})]
		public SourceNestedSelectRowset(SelectQuery select, Optional<AliasName> aliasName): base(aliasName) {
			if (select == null) {
				throw new ArgumentNullException("select");
			}
			this.@select = select;
		}

		public SelectQuery Select {
			get {
				return select;
			}
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.WriteLine("(");
			writer.WriteScript(select);
			writer.Write(")");
			base.WriteTo(writer);
		}
	}
}
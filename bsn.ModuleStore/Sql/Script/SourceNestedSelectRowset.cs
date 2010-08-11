using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceNestedSelectRowset: SourceRowset {
		private readonly SelectQuery @select;

		[Rule("<SourceRowset> ::= '(' <SelectQuery> ')' <OptionalAlias>", ConstructorParameterMapping = new[] {1, 3})]
		public SourceNestedSelectRowset(SelectQuery select, Optional<AliasName> aliasName): base(aliasName) {
			Debug.Assert(select != null);
			this.@select = select;
		}

		public SelectQuery Select {
			get {
				return select;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteLine("(");
			writer.WriteScript(select);
			writer.Write(")");
			base.WriteTo(writer);
		}
	}
}
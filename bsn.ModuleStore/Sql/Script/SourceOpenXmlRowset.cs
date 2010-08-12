using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceOpenxmlRowset: SourceRowset {
		private readonly OpenxmlFunction openxml;

		[Rule("<SourceRowset> ::= <Openxml> <OptionalAlias>")]
		public SourceOpenxmlRowset(OpenxmlFunction openxml, Optional<AliasName> aliasName): base(aliasName) {
			Debug.Assert(openxml != null);
			this.openxml = openxml;
		}

		public OpenxmlFunction Openxml {
			get {
				return openxml;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(openxml, WhitespacePadding.None);
			base.WriteTo(writer);
		}
	}
}
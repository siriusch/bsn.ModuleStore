using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceOpenxmlRowset: SourceRowset {
		private readonly OpenxmlFunction openxml;

		[Rule("<SourceRowset> ::= <Openxml> <OptionalAlias>")]
		public SourceOpenxmlRowset(OpenxmlFunction openxml, Optional<AliasName> aliasName): base(aliasName) {
			if (openxml == null) {
				throw new ArgumentNullException("openxml");
			}
			this.openxml = openxml;
		}

		public OpenxmlFunction Openxml {
			get {
				return openxml;
			}
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.WriteScript(openxml);
			base.WriteTo(writer);
		}
	}
}
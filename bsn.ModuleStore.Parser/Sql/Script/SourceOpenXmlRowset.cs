using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceOpenxmlRowset: SourceRowset {
		private readonly OpenxmlFunction openxml;

		[Rule("<SourceRowset> ::= <Openxml> <RowsetAlias>")]
		public SourceOpenxmlRowset(OpenxmlFunction openxml, RowsetAlias rowsetAlias): base(rowsetAlias) {
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
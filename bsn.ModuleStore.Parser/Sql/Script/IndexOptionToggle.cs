using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class IndexOptionToggle: IndexOption {
		private readonly bool value;

		[Rule("<IndexOption> ::= Id ~'=' <Toggle>")]
		public IndexOptionToggle(Identifier key, ToggleToken value): base(key) {
			Debug.Assert(value != null);
			this.value = value.On;
		}

		public bool Value {
			get {
				return value;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteToggle(value, WhitespacePadding.None);
		}
	}
}
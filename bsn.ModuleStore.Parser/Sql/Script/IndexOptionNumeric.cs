using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class IndexOptionNumeric: IndexOption {
		private readonly IntegerLiteral value;

		[Rule("<IndexOption> ::= Id ~'=' <IntegerLiteral>")]
		public IndexOptionNumeric(Identifier key, IntegerLiteral value): base(key) {
			Debug.Assert(value != null);
			this.value = value;
		}

		public IntegerLiteral Value {
			get {
				return value;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteScript(value, WhitespacePadding.None);
		}
	}
}
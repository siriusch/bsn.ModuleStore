using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class IndexOptionNumeric: IndexOption {
		private readonly IntegerLiteral value;

		[Rule("<IndexOption> ::= Id '=' <IntegerLiteral>", ConstructorParameterMapping = new[] {0, 2})]
		public IndexOptionNumeric(Identifier key, IntegerLiteral value): base(key) {
			if (value == null) {
				throw new ArgumentNullException("value");
			}
			this.value = value;
		}

		public IntegerLiteral Value {
			get {
				return value;
			}
		}

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			writer.WriteScript(value);
		}
	}
}
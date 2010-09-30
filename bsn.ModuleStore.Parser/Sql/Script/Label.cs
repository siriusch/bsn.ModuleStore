using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class Label: Statement {
		private readonly string identifier;

		[Rule("<Label> ::= Id ~':'")]
		public Label(Identifier identifier) {
			this.identifier = identifier.Value;
		}

		public string Identifier {
			get {
				return identifier;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write(identifier);
		}
	}
}
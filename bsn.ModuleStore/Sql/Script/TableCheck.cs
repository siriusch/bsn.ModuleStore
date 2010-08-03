using System;
using System.IO;
using System.Linq;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TableCheck: SqlToken {
		private bool? check;

		[Rule("<TableCheck> ::=")]
		public TableCheck() {}

		[Rule("<TableCheck> ::= WITH CHECK", ConstructorParameterMapping = new[] {1})]
		[Rule("<TableCheck> ::= WITH NOCHECK", ConstructorParameterMapping = new[] {1})]
		public TableCheck(IToken check) {
			this.check = check.Symbol.Name.Equals("CHECK", StringComparison.Ordinal);
		}

		public bool? Check {
			get {
				return check;
			}
		}

		public override void WriteTo(TextWriter writer) {
			if (check.HasValue) {
				writer.Write(" WITH ");
				writer.Write(check.Value ? "CHECK" : "NOCHECK");
			}
		}
	}
}
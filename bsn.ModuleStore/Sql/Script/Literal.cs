using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("HexLiteral")]
	[Terminal("IntegerLiteral")]
	[Terminal("RealLiteral")]
	[Terminal("StringLiteral")]
	public class Literal: SqlToken {
		private readonly string value;

		public Literal(string value) {
			this.value = value;
		}

		public string Value {
			get {
				return value;
			}
		}

		public override string ToString() {
			return Value;
		}
	}
}

using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class WaitforStatement: Statement {
		private readonly Identifier identifier;
		private readonly SqlScriptableToken stringValue;

		[Rule("<WaitforStatement> ::= ~WAITFOR Id <StringLiteral>")]
		[Rule("<WaitforStatement> ::= ~WAITFOR Id <VariableName>")]
		public WaitforStatement(Identifier identifier, SqlScriptableToken stringValue) {
			Debug.Assert(identifier != null);
			Debug.Assert(stringValue != null);
			this.identifier = identifier;
			this.stringValue = stringValue;
		}

		public Identifier Identifier {
			get {
				return identifier;
			}
		}

		public SqlScriptableToken StringValue {
			get {
				return stringValue;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("WAIT FOR ");
			writer.WriteScript(identifier, WhitespacePadding.SpaceAfter);
			stringValue.WriteTo(writer);
		}
	}
}
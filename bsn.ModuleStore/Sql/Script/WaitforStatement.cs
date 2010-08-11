using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class WaitforStatement: Statement {
		private readonly Identifier identifier;
		private readonly IScriptable stringValue;

		[Rule("<WaitforStatement> ::= WAITFOR Id <StringLiteral>", ConstructorParameterMapping = new[] {1, 2})]
		[Rule("<WaitforStatement> ::= WAITFOR Id <VariableName>", ConstructorParameterMapping = new[] {1, 2})]
		public WaitforStatement(Identifier identifier, IScriptable stringValue) {
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

		public IScriptable StringValue {
			get {
				return stringValue;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("WAIT FOR ");
			writer.WriteScript(identifier, WhitespacePadding.SpaceAfter);
			stringValue.WriteTo(writer);
		}
	}
}
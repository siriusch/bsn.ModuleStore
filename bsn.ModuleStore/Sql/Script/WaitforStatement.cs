using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class WaitforStatement: Statement {
		private readonly Identifier identifier;
		private readonly IScriptable stringValue;

		[Rule("<WaitforStatement> ::= WAITFOR Id <StringLiteral>", ConstructorParameterMapping = new[] {1, 2})]
		[Rule("<WaitforStatement> ::= WAITFOR Id <VariableName>", ConstructorParameterMapping=new[] { 1, 2 })]
		public WaitforStatement(Identifier identifier, IScriptable stringValue) {
			if (identifier == null) {
				throw new ArgumentNullException("identifier");
			}
			if (stringValue == null) {
				throw new ArgumentNullException("stringValue");
			}
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

		public override void WriteTo(TextWriter writer) {
			writer.Write("WAIT FOR ");
			writer.WriteScript(identifier, null, " ");
			stringValue.WriteTo(writer);
		}
	}
}
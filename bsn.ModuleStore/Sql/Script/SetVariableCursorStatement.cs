using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SetVariableCursorStatement: SetVariableStatement {
		private readonly CursorDefinition definition;

		[Rule("<SetVariableStatement> ::= SET <VariableName> '=' <CursorDefinition>", ConstructorParameterMapping = new[] {1, 3})]
		public SetVariableCursorStatement(VariableName variableName, CursorDefinition definition): base(variableName) {
			if (definition.Global) {
				throw new ArgumentException("GLOBAL not allowed here");
			}
			this.definition = definition;
		}

		public CursorDefinition Definition {
			get {
				return definition;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteScript(Definition, WhitespacePadding.None);
		}
	}
}
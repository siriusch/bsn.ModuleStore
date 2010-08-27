using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DeclareCursorStatement: CursorStatement {
		private readonly CursorDefinition definition;

		[Rule("<DeclareStatement> ::= DECLARE <CursorName> <CursorDefinition>", ConstructorParameterMapping = new[] {1, 2})]
		public DeclareCursorStatement(CursorName cursorName, CursorDefinition definition): base(definition.Global ? cursorName.AsGlobal() : cursorName) {
			this.definition = definition;
		}

		public CursorDefinition Definition {
			get {
				return definition;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("DECLARE ");
			CursorName.WriteNonGlobalInternal(writer);
			writer.WriteScript(definition, WhitespacePadding.SpaceBefore);
		}
	}
}
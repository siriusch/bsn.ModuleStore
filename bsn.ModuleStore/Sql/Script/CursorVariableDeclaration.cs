using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CursorVariableDeclaration: DeclareStatement {
		[Rule("<DeclareItem> ::= <VariableName> CURSOR", ConstructorParameterMapping = new[] {0})]
		public CursorVariableDeclaration(VariableName variable): base(variable) {}

		public override void WriteTo(TextWriter writer) {
			writer.Write("DECLARE ");
			VariableName.WriteTo(writer);
			writer.Write(" CURSOR");
		}
	}
}
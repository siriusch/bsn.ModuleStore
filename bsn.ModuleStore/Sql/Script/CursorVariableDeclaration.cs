using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CursorVariableDeclaration: VariableDeclaration {
		[Rule("<DeclareItem> ::= <VariableName> CURSOR", ConstructorParameterMapping = new int[0])]
		public CursorVariableDeclaration(VariableName variable): base(variable) {}

		public override void WriteTo(TextWriter writer) {
			writer.Write("DECLARE ");
			Variable.WriteTo(writer);
			writer.Write(" CURSOR");
		}
	}
}
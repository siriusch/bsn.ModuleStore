using System;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DeclareVariableStatement: SqlStatement {
		private readonly Sequence<VariableDeclaration> declarations;

		[Rule("<DeclareStatement> ::= DECLARE <DeclareItemList>", ConstructorParameterMapping = new[] {1})]
		public DeclareVariableStatement(Sequence<VariableDeclaration> declarations) {
			if (declarations == null) {
				throw new ArgumentNullException("declarations");
			}
			this.declarations = declarations;
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("DECLARE");
			string prepend = " ";
			foreach (VariableDeclaration declaration in declarations) {
				writer.Write(prepend);
				declaration.WriteTo(writer);
				prepend = ", ";
			}
		}
	}
}
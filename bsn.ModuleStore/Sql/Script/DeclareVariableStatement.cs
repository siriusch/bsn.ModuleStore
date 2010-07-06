using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bsn.ModuleStore.Sql.Script {
	public class DeclareVariableStatement: SqlStatement {
		private readonly Sequence<VariableDeclaration> declarations;

		public DeclareVariableStatement(Sequence<VariableDeclaration> declarations) {
			if (declarations == null) {
				throw new ArgumentNullException("declarations");
			}
			this.declarations = declarations;
		}

		public override void WriteTo(System.IO.TextWriter writer) {
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

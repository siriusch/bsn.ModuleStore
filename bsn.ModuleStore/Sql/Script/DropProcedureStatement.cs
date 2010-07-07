using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DropProcedureStatement: SqlDropStatement {
		private readonly ProcedureName procedureName;

		[Rule("<DropProcedureStatement> ::= DROP PROCEDURE <ProcedureName>", ConstructorParameterMapping = new[] {2})]
		public DropProcedureStatement(ProcedureName procedureName) {
			if (procedureName == null) {
				throw new ArgumentNullException("procedureName");
			}
			this.procedureName = procedureName;
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("DROP PROCEDURE ");
			procedureName.WriteTo(writer);
		}
	}
}

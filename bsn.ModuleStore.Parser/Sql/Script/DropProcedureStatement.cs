using System;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropProcedureStatement: DropStatement {
		private readonly Qualified<SchemaName, ProcedureName> procedureName;

		[Rule("<DropProcedureStatement> ::= DROP PROCEDURE <ProcedureNameQualified>", ConstructorParameterMapping = new[] {2})]
		public DropProcedureStatement(Qualified<SchemaName, ProcedureName> procedureName) {
			Debug.Assert(procedureName != null);
			this.procedureName = procedureName;
		}

		public Qualified<SchemaName, ProcedureName> ProcedureName {
			get {
				return procedureName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("DROP PROCEDURE ");
			writer.WriteScript(procedureName, WhitespacePadding.None);
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateProcedureStatement: SqlCreateStatement {
		private readonly StatementBlock body;
		private readonly bool forReplication;
		private readonly List<ProcedureParameter> parameters;
		private readonly ProcedureName procedureName;
		private readonly bool recompile;

		[Rule("<CreateProcedureStatement> ::= CREATE PROCEDURE <ProcedureName> <ProcedureParameterGroup> <ProcedureOptionGroup> <ProcedureFor> AS <StatementBlock>", ConstructorParameterMapping = new[] {2, 3, 4, 5, 7})]
		public CreateProcedureStatement(ProcedureName procedureName, Optional<Sequence<ProcedureParameter>> parameters, Optional<WithRecompileToken> recompile, Optional<ForReplicationToken> forReplication, StatementBlock body) {
			if (procedureName == null) {
				throw new ArgumentNullException("procedureName");
			}
			if (body == null) {
				throw new ArgumentNullException("body");
			}
			this.procedureName = procedureName;
			this.parameters = parameters.ToList();
			this.recompile = recompile.HasValue();
			this.forReplication = forReplication.HasValue();
			this.body = body;
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("CREATE PROCEDURE ");
			procedureName.WriteTo(writer);
			if (parameters != null) {
				string separator = " ";
				foreach (ProcedureParameter parameter in parameters) {
					writer.Write(separator);
					parameter.WriteTo(writer);
					separator = ", ";
				}
			}
			if (recompile) {
				writer.Write(" WITH RECOMPILE");
			}
			if (forReplication) {
				writer.Write(" FOR REPLICATION");
			}
			writer.Write(" AS ");
			body.WriteTo(writer);
		}
	}
}
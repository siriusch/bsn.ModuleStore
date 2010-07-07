using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateProcedureStatement: SqlCreateStatement {
		private readonly StatementBlock body;
		private readonly ForReplication forReplication;
		private readonly Sequence<ProcedureParameter> parameters;
		private readonly ProcedureName procedureName;
		private readonly WithRecompile recompile;

		[Rule("<CreateProcedureStatement> ::= CREATE PROCEDURE <ProcedureName> <ProcedureParameterGroup> <ProcedureOptionGroup> <ProcedureFor> AS <StatementBlock>", ConstructorParameterMapping = new[] {2, 3, 4, 5, 7})]
		public CreateProcedureStatement(ProcedureName procedureName, Optional<Sequence<ProcedureParameter>> parameters, Optional<WithRecompile> recompile, Optional<ForReplication> forReplication, StatementBlock body) {
			if (procedureName == null) {
				throw new ArgumentNullException("procedureName");
			}
			if (body == null) {
				throw new ArgumentNullException("body");
			}
			this.procedureName = procedureName;
			this.parameters = parameters;
			this.recompile = recompile;
			this.forReplication = forReplication;
			this.body = body;
		}

		public override void WriteTo(System.IO.TextWriter writer) {
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
			if (recompile != null) {
				writer.Write(' ');
				recompile.WriteTo(writer);
			}
			if (forReplication != null) {
				writer.Write(' ');
				forReplication.WriteTo(writer);
			}
			writer.Write(" AS ");
			body.WriteTo(writer);
		}
	}
}

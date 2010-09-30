using System;
using System.Collections.Generic;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TryCatchStatement: Statement {
		private readonly List<Statement> catchStatements;
		private readonly List<Statement> tryStatements;

		[Rule("<TryCatchStatement> ::= ~BEGIN ~TRY <StatementList> ~END ~TRY ~BEGIN ~CATCH <StatementList> ~END ~CATCH")]
		public TryCatchStatement(Sequence<Statement> tryStatements, Sequence<Statement> catchStatements) {
			this.tryStatements = tryStatements.ToList();
			this.catchStatements = catchStatements.ToList();
		}

		public IEnumerable<Statement> CatchStatements {
			get {
				return catchStatements;
			}
		}

		public IEnumerable<Statement> TryStatements {
			get {
				return tryStatements;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("BEGIN TRY");
			writer.IncreaseIndent();
			writer.WriteScriptSequence(tryStatements, WhitespacePadding.NewlineBefore, ";");
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.WriteLine("END TRY");
			writer.Write("BEGIN CATCH");
			writer.IncreaseIndent();
			writer.WriteScriptSequence(catchStatements, WhitespacePadding.NewlineBefore, ";");
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write("END TRY");
		}
	}
}
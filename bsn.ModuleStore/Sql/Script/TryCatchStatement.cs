using System;
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TryCatchStatement: Statement {
		private readonly List<Statement> catchStatements;
		private readonly List<Statement> tryStatements;

		[Rule("<TryCatchStatement> ::= BEGIN_TRY <StatementList> END_TRY BEGIN_CATCH <StatementList> END_CATCH", ConstructorParameterMapping = new[] {1, 4})]
		public TryCatchStatement(Sequence<Statement> tryStatements, Sequence<Statement> catchStatements) {
			if (tryStatements == null) {
				throw new ArgumentNullException("tryStatements");
			}
			if (catchStatements == null) {
				throw new ArgumentNullException("catchStatements");
			}
			this.tryStatements = tryStatements.ToList();
			this.catchStatements = catchStatements.ToList();
		}

		public List<Statement> CatchStatements {
			get {
				return catchStatements;
			}
		}

		public List<Statement> TryStatements {
			get {
				return tryStatements;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteLine("BEGIN TRY");
			writer.WriteSequence(tryStatements, "\t", null, ";"+Environment.NewLine);
			writer.WriteLine("END TRY");
			writer.WriteLine("BEGIN CATCH ");
			writer.WriteSequence(catchStatements, "\t", null, ";"+Environment.NewLine);
			writer.WriteLine("END TRY");
		}
	}
}
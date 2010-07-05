using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TryCatchStatement: SqlStatement {
		private readonly Sequence<SqlStatement> tryStatements;
		private readonly Sequence<SqlStatement> catchStatements;

		[Rule("<TryCatchStatement> ::= BEGIN_TRY <StatementList> END_TRY BEGIN_CATCH <StatementList> END_CATCH", ConstructorParameterMapping = new[] {1, 4})]
		public TryCatchStatement(Sequence<SqlStatement> tryStatements, Sequence<SqlStatement> catchStatements) {
			if (tryStatements == null) {
				throw new ArgumentNullException("tryStatements");
			}
			if (catchStatements == null) {
				throw new ArgumentNullException("catchStatements");
			}
			this.tryStatements = tryStatements;
			this.catchStatements = catchStatements;
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("BEGIN TRY ");
			tryStatements.WriteTo(writer);
			writer.Write(" END TRY BEGIN CATCH ");
			catchStatements.WriteTo(writer);
			writer.Write(" END TRY");
		}
	}
}
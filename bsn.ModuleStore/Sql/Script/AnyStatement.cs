using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class AnyStatement: Statement {
		private readonly string statementText;

		[Rule("<AnyStatement> ::= Id <ExpressionList>")]
		public AnyStatement(Identifier identifier, Sequence<Expression> expressions) {
			using (StringWriter stringWriter = new StringWriter()) {
				SqlWriter statementWriter = new SqlWriter(stringWriter);
				statementWriter.Write(identifier.Value);
				statementWriter.WriteSequence(expressions, WhitespacePadding.SpaceBefore, null);
				statementText = stringWriter.ToString();
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteLine(statementText);
		}
	}
}
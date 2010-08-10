using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class AnyStatement: Statement {
		private readonly string statementText;

		[Rule("<AnyStatement> ::= Id <ExpressionList>")]
		public AnyStatement(Identifier identifier, Sequence<Expression> expressions) {
			using (StringWriter statementWriter = CreateWriter()) {
				statementWriter.Write(identifier.Value);
				statementWriter.WriteSequence(expressions, " ", string.Empty, string.Empty);
				statementText = statementWriter.ToString();
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteLine(statementText);
		}
	}
}
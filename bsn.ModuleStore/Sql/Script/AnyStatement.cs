using System;
using System.Globalization;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class AnyStatement: SqlStatement {
		private readonly string statementText;

		[Rule("Id <ExpressionList>")]
		public AnyStatement(Identifier identifier, Sequence<Expression> expressions) {
			using (StringWriter statementWriter = CreateWriter()) {
				statementWriter.Write(identifier.Value);
				foreach (Expression expression in expressions) {
					statementWriter.Write(' ');
					expression.WriteTo(statementWriter);
				}
				statementText = statementWriter.ToString();
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write(statementText);
		}
	}
}
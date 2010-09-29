using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SetOptionStatement: Statement {
		private readonly string option;

		[Rule("<SetOptionStatement> ::= ~SET ROWCOUNT <SetValue>")]
		public SetOptionStatement(SqlScriptableToken identifier, SqlScriptableToken value): this(identifier, new Sequence<SqlScriptableToken>(value)) {}

		[Rule("<SetOptionStatement> ::= ~SET Id <SetValueList>")]
		[Rule("<SetOptionStatement> ::= ~SET TRANSACTION <SetValueList>")]
		[Rule("<SetOptionStatement> ::= ~SET OFFSETS <SetValueList>")]
		[Rule("<SetOptionStatement> ::= ~SET STATISTICS <SetValueList>")]
		public SetOptionStatement(SqlScriptableToken identifier, Sequence<SqlScriptableToken> valueList) {
			using (StringWriter stringWriter = new StringWriter()) {
				SqlWriter writer = new SqlWriter(stringWriter);
				writer.WriteScript(identifier, WhitespacePadding.None);
				foreach (SqlScriptableToken token in valueList) {
					writer.Write(' ');
					token.WriteTo(writer);
				}
				option = stringWriter.ToString();
			}
		}

		public string Option {
			get {
				return option;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("SET ");
			writer.Write(option);
		}
	}
}
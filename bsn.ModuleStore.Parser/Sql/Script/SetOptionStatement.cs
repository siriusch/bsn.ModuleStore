using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SetOptionStatement: Statement {
		private readonly string option;

		[Rule("<SetOptionStatement> ::= SET Id <SetValueList>", ConstructorParameterMapping = new[] {1, 2})]
		public SetOptionStatement(Identifier identifier, Sequence<SqlScriptableToken> valueList) {
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
			writer.Write("SET ");
			writer.Write(option);
		}
	}
}
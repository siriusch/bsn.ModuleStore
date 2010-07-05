using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class SetOptionStatement: SqlStatement {
		private readonly string setOption;

		[Rule("<SetOptionStatement> ::= SET Id <SetValueList>", ConstructorParameterMapping = new[] {1, 2})]
		public SetOptionStatement(Identifier identifier, Sequence<SqlToken> valueList) {
			using (StringWriter writer = CreateWriter()) {
				writer.Write("SET ");
				identifier.WriteTo(writer);
				foreach (SqlToken token in valueList) {
					writer.Write(' ');
					token.WriteTo(writer);
				}
				setOption = writer.ToString();
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write(setOption);
		}
	}
}
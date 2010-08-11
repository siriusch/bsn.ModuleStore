using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DeclareCursorItem: DeclareItem {
		[Rule("<DeclareItem> ::= <VariableName> CURSOR", ConstructorParameterMapping = new[] {0})]
		public DeclareCursorItem(VariableName variable): base(variable) {}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("CURSOR");
		}
	}
}